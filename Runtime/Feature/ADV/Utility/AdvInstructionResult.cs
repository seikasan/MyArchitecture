namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvInstructionResult
    {
        private AdvInstructionResult(
            AdvInstructionFlow flow,
            string targetLabel)
        {
            Flow = flow;
            TargetLabel = targetLabel;
        }

        public AdvInstructionFlow Flow { get; }
        public string TargetLabel { get; }

        public static AdvInstructionResult Continue()
            => new(AdvInstructionFlow.Continue, null);

        public static AdvInstructionResult Pause()
            => new(AdvInstructionFlow.Pause, null);

        public static AdvInstructionResult Jump(string targetLabel)
            => new(AdvInstructionFlow.Jump, targetLabel);

        public static AdvInstructionResult End()
            => new(AdvInstructionFlow.End, null);
    }
}