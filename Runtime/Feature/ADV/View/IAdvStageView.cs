using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvStageView : IView
    {
        void ApplySignal(AdvSignal signal);
    }
}