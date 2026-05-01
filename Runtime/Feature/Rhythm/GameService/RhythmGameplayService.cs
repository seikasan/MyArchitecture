using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class RhythmGameplayService : GameService
    {
        private const double ScheduleLeadTime = 0.1d;

        private readonly RhythmPlaybackModel _playbackModel;
        private readonly RhythmNoteModel _noteModel;
        private readonly RhythmScoreModel _scoreModel;
        private readonly IReadOnlyList<IRhythmChartProvider> _chartProviders;
        private readonly IRhythmClock _clock;
        private readonly IRhythmJudgeEvaluator _judgeEvaluator;
        private readonly IRhythmScoreCalculator _scoreCalculator;
        private readonly HashSet<RhythmNoteId> _judgedNoteIds = new();
        private readonly Dictionary<RhythmNoteId, RhythmNote> _activeHoldNotes = new();

        private CancellationTokenSource _loopCancellationTokenSource;

        public RhythmGameplayService(
            RhythmPlaybackModel playbackModel,
            RhythmNoteModel noteModel,
            RhythmScoreModel scoreModel,
            IEnumerable<IRhythmChartProvider> chartProviders,
            IEnumerable<IRhythmClock> clocks,
            IEnumerable<IRhythmJudgeEvaluator> judgeEvaluators,
            IEnumerable<IRhythmScoreCalculator> scoreCalculators)
        {
            _playbackModel = playbackModel;
            _noteModel = noteModel;
            _scoreModel = scoreModel;
            _chartProviders = chartProviders?.ToArray() ??
                              Array.Empty<IRhythmChartProvider>();
            _clock = SelectUtility(
                clocks,
                typeof(UnityAudioDspRhythmClock),
                nameof(IRhythmClock));
            _judgeEvaluator = SelectUtility(
                judgeEvaluators,
                typeof(DefaultRhythmJudgeEvaluator),
                nameof(IRhythmJudgeEvaluator));
            _scoreCalculator = SelectUtility(
                scoreCalculators,
                typeof(DefaultRhythmScoreCalculator),
                nameof(IRhythmScoreCalculator));
        }

        public async UniTask StartAsync(
            string chartId,
            CancellationToken cancellationToken)
        {
            RhythmChart chart = await LoadChartAsync(
                chartId,
                cancellationToken);

            Start(chart);
        }

        public void Start(RhythmChart chart)
        {
            if (chart == null)
            {
                throw new ArgumentNullException(nameof(chart));
            }

            StopLoop();
            ResetRuntimeState();

            double scheduledDspStartTime = _clock.DspTime + ScheduleLeadTime;

            _playbackModel.Start(
                chart,
                scheduledDspStartTime);
            _scoreModel.ResetScore();

            this.PublishEvent(new RhythmChartLoadedEvent(chart));
            this.PublishEvent(
                new RhythmPlayScheduledEvent(
                    chart.ChartId,
                    chart.MusicKey,
                    scheduledDspStartTime,
                    chart.ChartOffset));
            this.PublishEvent(new RhythmStartedEvent(chart.ChartId));

            StartLoop();
        }

        public void SubmitInput(RhythmInput input)
        {
            if (_playbackModel.PlaybackState != RhythmPlaybackState.Playing ||
                _playbackModel.CurrentChart == null)
            {
                return;
            }

            double inputDspTime = input.HasDspTime
                ? input.DspTime
                : _clock.DspTime;
            RhythmInput stampedInput = input.WithDspTime(inputDspTime);
            double chartTime = ToChartTime(inputDspTime) + _playbackModel.InputOffset;

            if (input.Kind == RhythmInputKind.Release &&
                TryJudgeActiveHold(stampedInput, chartTime))
            {
                return;
            }

            var candidates = GetUnjudgedNotes()
                .Where(note => !_activeHoldNotes.ContainsKey(note.NoteId))
                .ToArray();
            RhythmJudgeResult result = _judgeEvaluator.Evaluate(
                new RhythmJudgeRequest(
                    stampedInput,
                    chartTime,
                    candidates,
                    _playbackModel.CurrentChart.JudgeProfile));

            if (!result.IsJudged)
            {
                return;
            }

            if (result.Note.Kind == RhythmNoteKind.Hold &&
                stampedInput.Kind == RhythmInputKind.Press &&
                result.IsHit)
            {
                _activeHoldNotes[result.NoteId] = result.Note;
                ApplyJudgeResult(result);
                return;
            }

            FinishNote(result.NoteId);
            ApplyJudgeResult(result);
        }

        public void Pause()
        {
            if (_playbackModel.PlaybackState != RhythmPlaybackState.Playing)
            {
                return;
            }

            UpdateChartTime();
            _playbackModel.SetPlaybackState(RhythmPlaybackState.Paused);
            StopLoop();

            this.PublishEvent(
                new RhythmPausedEvent(
                    _playbackModel.CurrentChartId,
                    _playbackModel.ChartTime));
        }

        public void Resume()
        {
            if (_playbackModel.PlaybackState != RhythmPlaybackState.Paused)
            {
                return;
            }

            double scheduledDspStartTime =
                _clock.DspTime + _playbackModel.ChartOffset - _playbackModel.ChartTime;

            _playbackModel.SetScheduledDspStartTime(scheduledDspStartTime);
            _playbackModel.SetPlaybackState(RhythmPlaybackState.Playing);

            this.PublishEvent(
                new RhythmResumedEvent(
                    _playbackModel.CurrentChartId,
                    _playbackModel.ChartTime,
                    scheduledDspStartTime));

            StartLoop();
        }

        public void Stop()
        {
            if (_playbackModel.PlaybackState == RhythmPlaybackState.Stopped)
            {
                return;
            }

            string chartId = _playbackModel.CurrentChartId;

            StopLoop();
            ResetRuntimeState();
            _playbackModel.Stop();

            this.PublishEvent(new RhythmStoppedEvent(chartId));
        }

        public void Seek(double chartTime)
        {
            if (_playbackModel.CurrentChart == null)
            {
                return;
            }

            double clampedChartTime = Math.Max(
                0d,
                Math.Min(chartTime, _playbackModel.CurrentChart.Length));
            double scheduledDspStartTime =
                _clock.DspTime + _playbackModel.ChartOffset - clampedChartTime;
            bool wasPlaying =
                _playbackModel.PlaybackState == RhythmPlaybackState.Playing;

            StopLoop();
            ResetRuntimeState();
            _playbackModel.SetChartTime(clampedChartTime);
            _playbackModel.SetScheduledDspStartTime(scheduledDspStartTime);

            if (wasPlaying)
            {
                _playbackModel.SetPlaybackState(RhythmPlaybackState.Playing);
                StartLoop();
            }
        }

        public bool CanSubmitInput()
        {
            return _playbackModel.PlaybackState == RhythmPlaybackState.Playing;
        }

        protected override void OnDispose()
        {
            StopLoop();
            base.OnDispose();
        }

        private async UniTask RunLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested &&
                       _playbackModel.PlaybackState == RhythmPlaybackState.Playing)
                {
                    UpdatePlaybackFrame();

                    await UniTask.Yield(
                        PlayerLoopTiming.Update,
                        cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void UpdatePlaybackFrame()
        {
            UpdateChartTime();
            SpawnVisibleNotes();
            DetectMisses();

            RhythmChart chart = _playbackModel.CurrentChart;

            if (chart == null)
            {
                return;
            }

            if (_judgedNoteIds.Count >= chart.Notes.Count &&
                _playbackModel.ChartTime >= chart.Length)
            {
                End();
            }
        }

        private void SpawnVisibleNotes()
        {
            RhythmChart chart = _playbackModel.CurrentChart;

            if (chart == null)
            {
                return;
            }

            foreach (RhythmNote note in chart.Notes)
            {
                if (_judgedNoteIds.Contains(note.NoteId) ||
                    _noteModel.Contains(note.NoteId))
                {
                    continue;
                }

                if (note.HitTime - _playbackModel.ChartTime > chart.LookAheadTime)
                {
                    continue;
                }

                if (_playbackModel.ChartTime > note.EndTime + chart.JudgeProfile.MissWindow)
                {
                    continue;
                }

                _noteModel.Add(
                    note.NoteId,
                    new RhythmNoteViewData(
                        note,
                        _playbackModel.ScheduledDspStartTime,
                        _playbackModel.ChartOffset));
            }
        }

        private void DetectMisses()
        {
            RhythmChart chart = _playbackModel.CurrentChart;

            if (chart == null)
            {
                return;
            }

            foreach (RhythmNote note in chart.Notes)
            {
                if (_judgedNoteIds.Contains(note.NoteId))
                {
                    continue;
                }

                double targetTime = note.EndTime;

                if (_playbackModel.ChartTime <= targetTime + chart.JudgeProfile.MissWindow)
                {
                    continue;
                }

                _activeHoldNotes.Remove(note.NoteId);
                FinishNote(note.NoteId);
                ApplyJudgeResult(
                    RhythmJudgeResult.Miss(
                        note,
                        _playbackModel.ChartTime - targetTime));
            }
        }

        private bool TryJudgeActiveHold(
            RhythmInput input,
            double chartTime)
        {
            var candidates = _activeHoldNotes.Values
                .Where(note => note.EndLaneId == input.LaneId)
                .ToArray();

            RhythmJudgeResult result = _judgeEvaluator.Evaluate(
                new RhythmJudgeRequest(
                    input,
                    chartTime,
                    candidates,
                    _playbackModel.CurrentChart.JudgeProfile));

            if (!result.IsJudged)
            {
                return false;
            }

            _activeHoldNotes.Remove(result.NoteId);
            FinishNote(result.NoteId);
            ApplyJudgeResult(result);

            return true;
        }

        private void ApplyJudgeResult(RhythmJudgeResult result)
        {
            this.PublishEvent(new RhythmInputJudgedEvent(result));

            RhythmScoreSnapshot snapshot = _scoreCalculator.Apply(
                _scoreModel.Snapshot(),
                result);

            _scoreModel.ApplySnapshot(snapshot);

            this.PublishEvent(new RhythmScoreChangedEvent(snapshot));
            this.PublishEvent(
                new RhythmComboChangedEvent(
                    snapshot.Combo,
                    snapshot.MaxCombo));
        }

        private void FinishNote(RhythmNoteId noteId)
        {
            _judgedNoteIds.Add(noteId);
            _noteModel.TryRemove(noteId);
        }

        private IEnumerable<RhythmNote> GetUnjudgedNotes()
        {
            RhythmChart chart = _playbackModel.CurrentChart;

            if (chart == null)
            {
                return Enumerable.Empty<RhythmNote>();
            }

            return chart.Notes
                .Where(note => !_judgedNoteIds.Contains(note.NoteId));
        }

        private void UpdateChartTime()
        {
            _playbackModel.SetChartTime(ToChartTime(_clock.DspTime));
        }

        private double ToChartTime(double dspTime)
        {
            return dspTime -
                   _playbackModel.ScheduledDspStartTime +
                   _playbackModel.ChartOffset;
        }

        private async UniTask<RhythmChart> LoadChartAsync(
            string chartId,
            CancellationToken cancellationToken)
        {
            IRhythmChartProvider provider = _chartProviders
                .FirstOrDefault(item => item.CanProvide(chartId));

            if (provider == null)
            {
                throw new KeyNotFoundException(
                    $"Rhythm chart provider is not found: {chartId}");
            }

            return await provider.LoadAsync(
                chartId,
                cancellationToken);
        }

        private void StartLoop()
        {
            StopLoop();
            _loopCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    DisposeCancellationToken);
            RunLoopAsync(_loopCancellationTokenSource.Token).Forget();
        }

        private void StopLoop()
        {
            if (_loopCancellationTokenSource == null)
            {
                return;
            }

            _loopCancellationTokenSource.Cancel();
            _loopCancellationTokenSource.Dispose();
            _loopCancellationTokenSource = null;
        }

        private void ResetRuntimeState()
        {
            _judgedNoteIds.Clear();
            _activeHoldNotes.Clear();
            _noteModel.Clear();
        }

        private void End()
        {
            StopLoop();
            _noteModel.Clear();
            _playbackModel.End();

            this.PublishEvent(
                new RhythmEndedEvent(
                    _playbackModel.CurrentChartId,
                    _scoreModel.Snapshot()));
        }

        private static T SelectUtility<T>(
            IEnumerable<T> utilities,
            Type defaultType,
            string utilityName)
            where T : class
        {
            var items = utilities?.Where(item => item != null).ToArray() ??
                        Array.Empty<T>();
            var customItems = items
                .Where(item => item.GetType() != defaultType)
                .ToArray();

            if (customItems.Length > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple custom {utilityName} implementations are registered.");
            }

            if (customItems.Length == 1)
            {
                return customItems[0];
            }

            T defaultItem = items.FirstOrDefault(item => item.GetType() == defaultType);

            if (defaultItem != null)
            {
                return defaultItem;
            }

            throw new InvalidOperationException(
                $"{utilityName} implementation is not registered.");
        }
    }
}
