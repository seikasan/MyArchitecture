namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvLabelInstruction : IAdvInstruction
    {
        public AdvLabelInstruction(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}