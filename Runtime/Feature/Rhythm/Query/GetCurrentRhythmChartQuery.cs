using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class GetCurrentRhythmChartQuery :
        Query<RhythmChart>
    {
        private readonly IReadOnlyRhythmPlaybackModel _playbackModel;

        public GetCurrentRhythmChartQuery(
            IReadOnlyRhythmPlaybackModel playbackModel)
        {
            _playbackModel = playbackModel;
        }

        protected override RhythmChart OnExecute()
        {
            return _playbackModel.CurrentChart;
        }
    }
}
