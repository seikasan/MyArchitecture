using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvDialogueView : IView
    {
        void ShowLine(AdvLine line);

        void ClearLine();
    }
}