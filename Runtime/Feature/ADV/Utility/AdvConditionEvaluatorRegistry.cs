using System;
using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvConditionEvaluatorRegistry : IUtility
    {
        void Register(IAdvConditionEvaluator evaluator);

        bool Evaluate(
            string customKey,
            AdvStateSnapshot state);
    }

    public sealed class AdvConditionEvaluatorRegistry :
        Utility,
        IAdvConditionEvaluatorRegistry
    {
        private readonly List<IAdvConditionEvaluator> _evaluators = new();

        public AdvConditionEvaluatorRegistry(
            IEnumerable<IAdvConditionEvaluator> evaluators = null)
        {
            foreach (var evaluator in evaluators ?? Array.Empty<IAdvConditionEvaluator>())
            {
                Register(evaluator);
            }
        }

        public void Register(IAdvConditionEvaluator evaluator)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            if (!_evaluators.Contains(evaluator))
            {
                _evaluators.Add(evaluator);
            }
        }

        public bool Evaluate(
            string customKey,
            AdvStateSnapshot state)
        {
            foreach (var evaluator in _evaluators)
            {
                if (!evaluator.CanEvaluate(customKey))
                {
                    continue;
                }

                return evaluator.Evaluate(customKey, state);
            }

            return false;
        }
    }
}