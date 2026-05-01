using System;
using System.Collections.Generic;
using System.Linq;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvScenario
    {
        private readonly IReadOnlyList<IAdvInstruction> _instructions;

        public AdvScenario(
            string scenarioId,
            string entryLabel,
            IEnumerable<IAdvInstruction> instructions)
        {
            if (string.IsNullOrWhiteSpace(scenarioId))
            {
                throw new ArgumentException(
                    "Scenario id is null or empty.",
                    nameof(scenarioId));
            }

            ScenarioId = scenarioId;
            EntryLabel = entryLabel;
            _instructions = instructions?.ToArray() ??
                            Array.Empty<IAdvInstruction>();
        }

        public string ScenarioId { get; }
        public string EntryLabel { get; }
        public IReadOnlyList<IAdvInstruction> Instructions => _instructions;

        public int GetEntryIndex()
        {
            return string.IsNullOrWhiteSpace(EntryLabel)
                ? 0
                : FindLabelIndex(EntryLabel);
        }

        public int FindLabelIndex(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException(
                    "Label is null or empty.",
                    nameof(label));
            }

            for (int i = 0; i < _instructions.Count; i++)
            {
                if (_instructions[i] is AdvLabelInstruction labelInstruction &&
                    labelInstruction.Label == label)
                {
                    return i + 1;
                }
            }

            throw new KeyNotFoundException(
                $"ADV label is not found: {label}");
        }
    }
}