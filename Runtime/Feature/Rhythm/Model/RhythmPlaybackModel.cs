using System;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public partial class RhythmPlaybackModel : Model
    {
        public RhythmChart CurrentChart { get; private set; }
        public string CurrentChartId { get; private set; }
        public RhythmPlaybackState PlaybackState { get; private set; } =
            RhythmPlaybackState.Stopped;
        public double ScheduledDspStartTime { get; private set; }
        public double ChartTime { get; private set; }
        public double ChartOffset { get; private set; }
        public double InputOffset { get; private set; }

        public void LoadChart(RhythmChart chart)
        {
            CurrentChart = chart ?? throw new ArgumentNullException(nameof(chart));
            CurrentChartId = chart.ChartId;
            ChartOffset = chart.ChartOffset;
            ChartTime = 0d;
            ScheduledDspStartTime = 0d;
            PlaybackState = RhythmPlaybackState.Stopped;
        }

        public void Start(
            RhythmChart chart,
            double scheduledDspStartTime)
        {
            CurrentChart = chart ?? throw new ArgumentNullException(nameof(chart));
            CurrentChartId = chart.ChartId;
            ChartOffset = chart.ChartOffset;
            ChartTime = 0d;
            ScheduledDspStartTime = scheduledDspStartTime;
            PlaybackState = RhythmPlaybackState.Playing;
        }

        public void SetPlaybackState(RhythmPlaybackState playbackState)
        {
            PlaybackState = playbackState;
        }

        public void SetChartTime(double chartTime)
        {
            ChartTime = Math.Max(0d, chartTime);
        }

        public void SetScheduledDspStartTime(double scheduledDspStartTime)
        {
            ScheduledDspStartTime = scheduledDspStartTime;
        }

        public void SetOffsets(
            double chartOffset,
            double inputOffset)
        {
            ChartOffset = chartOffset;
            InputOffset = inputOffset;
        }

        public void Stop()
        {
            PlaybackState = RhythmPlaybackState.Stopped;
            ChartTime = 0d;
            ScheduledDspStartTime = 0d;
        }

        public void End()
        {
            PlaybackState = RhythmPlaybackState.Ended;
        }
    }
}
