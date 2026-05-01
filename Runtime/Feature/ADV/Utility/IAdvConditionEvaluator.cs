using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvConditionEvaluator : IUtility
    {
        bool CanEvaluate(string customKey);

        bool Evaluate(
            string customKey,
            AdvStateSnapshot state);
    }
}