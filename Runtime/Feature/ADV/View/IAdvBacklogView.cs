using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvBacklogView : IView
    {
        void ShowBacklog(IReadOnlyList<AdvBacklogEntry> backlog);

        void HideBacklog();
    }
}