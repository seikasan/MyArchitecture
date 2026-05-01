using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmInputView : IView
    {
        ViewSignal<RhythmInput> InputSubmitted { get; }
        ViewSignal PauseRequested { get; }
        ViewSignal ResumeRequested { get; }
        ViewSignal StopRequested { get; }
    }
}
