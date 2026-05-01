using System;
using System.Collections.Generic;
using System.Linq;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public partial class AdvScenarioModel :
        Model
    {
        private readonly List<AdvChoice> _currentChoices = new();
        private readonly List<AdvBacklogEntry> _backlog = new();
        private readonly List<AdvChoiceHistoryEntry> _choiceHistory = new();

        public AdvScenario CurrentScenario { get; private set; }
        public string CurrentScenarioId { get; private set; }
        public int InstructionIndex { get; private set; }
        public AdvPlaybackState PlaybackState { get; private set; } =
            AdvPlaybackState.Stopped;
        public AdvLine CurrentLine { get; private set; }
        public AdvWaitRequest CurrentWait { get; private set; }
        public IReadOnlyList<AdvChoice> CurrentChoices => _currentChoices;
        public IReadOnlyList<AdvBacklogEntry> Backlog => _backlog;
        public IReadOnlyList<AdvChoiceHistoryEntry> ChoiceHistory => _choiceHistory;

        public void StartScenario(AdvScenario scenario, int instructionIndex)
        {
            CurrentScenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            CurrentScenarioId = scenario.ScenarioId;
            InstructionIndex = instructionIndex;
            PlaybackState = AdvPlaybackState.Running;
            CurrentLine = null;
            CurrentWait = null;
            _currentChoices.Clear();
            _backlog.Clear();
            _choiceHistory.Clear();
        }

        public void SetInstructionIndex(int instructionIndex)
        {
            InstructionIndex = instructionIndex;
        }

        public void SetPlaybackState(AdvPlaybackState playbackState)
        {
            PlaybackState = playbackState;
        }

        public void SetCurrentLine(AdvLine line)
        {
            CurrentLine = line;
            CurrentWait = null;
            _currentChoices.Clear();
        }

        public void SetCurrentChoices(IEnumerable<AdvChoice> choices)
        {
            CurrentLine = null;
            CurrentWait = null;
            _currentChoices.Clear();
            _currentChoices.AddRange(choices ?? Enumerable.Empty<AdvChoice>());
        }

        public void ClearCurrentChoices()
        {
            _currentChoices.Clear();
        }

        public void SetCurrentWait(AdvWaitRequest wait)
        {
            CurrentLine = null;
            CurrentWait = wait;
            _currentChoices.Clear();
        }

        public void AddBacklog(AdvBacklogEntry entry)
        {
            if (entry != null)
            {
                _backlog.Add(entry);
            }
        }

        public void AddChoiceHistory(AdvChoice choice)
        {
            if (choice == null)
            {
                return;
            }

            _choiceHistory.Add(
                new AdvChoiceHistoryEntry(
                    choice.ChoiceId,
                    choice.Label));

            _backlog.Add(
                new AdvBacklogEntry(
                    null,
                    choice.ChoiceId,
                    choice.Label));
        }

        public void EndScenario()
        {
            PlaybackState = AdvPlaybackState.Ended;
            CurrentLine = null;
            CurrentWait = null;
            _currentChoices.Clear();
        }

        public AdvScenarioSnapshot CreateSnapshot()
        {
            return new AdvScenarioSnapshot(
                CurrentScenarioId,
                InstructionIndex,
                PlaybackState,
                CurrentLine,
                _currentChoices,
                CurrentWait,
                _backlog,
                _choiceHistory);
        }

        public void RestoreSnapshot(
            AdvScenario scenario,
            AdvScenarioSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            CurrentScenario = scenario;
            CurrentScenarioId = snapshot.ScenarioId;
            InstructionIndex = snapshot.InstructionIndex;
            PlaybackState = snapshot.PlaybackState;
            CurrentLine = snapshot.CurrentLine;
            CurrentWait = snapshot.CurrentWait;

            _currentChoices.Clear();
            _currentChoices.AddRange(
                snapshot.CurrentChoices ?? Array.Empty<AdvChoice>());

            _backlog.Clear();
            _backlog.AddRange(
                snapshot.Backlog ?? Array.Empty<AdvBacklogEntry>());

            _choiceHistory.Clear();
            _choiceHistory.AddRange(
                snapshot.ChoiceHistory ?? Array.Empty<AdvChoiceHistoryEntry>());
        }
    }
}
