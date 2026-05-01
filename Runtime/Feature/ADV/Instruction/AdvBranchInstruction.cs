namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvBranchInstruction : IAdvInstruction
    {
        public AdvBranchInstruction(
            AdvCondition condition,
            string trueLabel,
            string falseLabel = null)
        {
            Condition = condition ?? AdvCondition.Always();
            TrueLabel = trueLabel;
            FalseLabel = falseLabel;
        }

        public AdvCondition Condition { get; }
        public string TrueLabel { get; }
        public string FalseLabel { get; }
    }
}