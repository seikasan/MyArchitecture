using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class CanSubmitRhythmInputQuery : Query<bool>
    {
        private readonly IReadOnlyRhythmPlaybackModel _playbackModel;

        public CanSubmitRhythmInputQuery(
            IReadOnlyRhythmPlaybackModel playbackModel)
        {
            _playbackModel = playbackModel;
        }

        protected override bool OnExecute()
        {
            return _playbackModel.PlaybackState == RhythmPlaybackState.Playing;
        }
    }
}
