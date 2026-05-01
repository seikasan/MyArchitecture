namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvJumpInstruction : IAdvInstruction
    {
        public AdvJumpInstruction(string targetLabel)
        {
            TargetLabel = targetLabel;
        }

        public string TargetLabel { get; }
    }
}