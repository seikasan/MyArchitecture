using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class GetRhythmPlaybackStateQuery :
        Query<RhythmPlaybackState>
    {
        private readonly IReadOnlyRhythmPlaybackModel _playbackModel;

        public GetRhythmPlaybackStateQuery(
            IReadOnlyRhythmPlaybackModel playbackModel)
        {
            _playbackModel = playbackModel;
        }

        protected override RhythmPlaybackState OnExecute()
        {
            return _playbackModel.PlaybackState;
        }
    }
}
