using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvChoiceView : IView
    {
        ViewSignal<string> ChoiceSelected { get; }

        void ShowChoices(IReadOnlyList<AdvChoice> choices);

        void ClearChoices();
    }
}