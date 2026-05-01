using System;
using System.Collections.Generic;
using System.Linq;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvChoiceInstruction : IAdvInstruction
    {
        private readonly IReadOnlyList<AdvChoice> _choices;

        public AdvChoiceInstruction(IEnumerable<AdvChoice> choices)
        {
            _choices = choices?.ToArray() ?? Array.Empty<AdvChoice>();
        }

        public IReadOnlyList<AdvChoice> Choices => _choices;
    }
}