namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvChoiceHistoryEntry
    {
        public AdvChoiceHistoryEntry(string choiceId, string label)
        {
            ChoiceId = choiceId;
            Label = label;
        }

        public string ChoiceId { get; }
        public string Label { get; }
    }
}