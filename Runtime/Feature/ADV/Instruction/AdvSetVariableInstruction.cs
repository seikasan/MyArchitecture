namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvSetVariableInstruction : IAdvInstruction
    {
        public AdvSetVariableInstruction(AdvVariableChange change)
        {
            Change = change;
        }

        public AdvVariableChange Change { get; }
    }
}