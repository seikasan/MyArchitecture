using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvInputView : IView
    {
        ViewSignal AdvanceRequested { get; }
        ViewSignal SkipRequested { get; }
        ViewSignal AutoRequested { get; }
        ViewSignal BacklogRequested { get; }
        ViewSignal SaveRequested { get; }
        ViewSignal LoadRequested { get; }
        void SetAdvanceEnabled(bool enabled);
    }
}