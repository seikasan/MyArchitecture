using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvWaitStartedEvent : IEvent
    {
        public AdvWaitStartedEvent(AdvWaitRequest wait)
        {
            Wait = wait;
        }

        public AdvWaitRequest Wait { get; }
    }
}