namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvLineInstruction : IAdvInstruction
    {
        public AdvLineInstruction(AdvLine line)
        {
            Line = line;
        }

        public AdvLine Line { get; }
    }
}