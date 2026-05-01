namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvChoice
    {
        public AdvChoice(
            string choiceId,
            string label,
            string targetLabel,
            string textKey = null,
            AdvCondition condition = null,
            string signal = null)
        {
            ChoiceId = choiceId;
            Label = label;
            TargetLabel = targetLabel;
            TextKey = textKey;
            Condition = condition;
            Signal = signal;
        }

        public string ChoiceId { get; }
        public string Label { get; }
        public string TargetLabel { get; }
        public string TextKey { get; }
        public AdvCondition Condition { get; }
        public string Signal { get; }
    }
}