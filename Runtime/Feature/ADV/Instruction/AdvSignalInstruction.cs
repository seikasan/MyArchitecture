namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvSignalInstruction : IAdvInstruction
    {
        public AdvSignalInstruction(AdvSignal signal)
        {
            Signal = signal;
        }

        public AdvSignal Signal { get; }
    }
}