namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvBacklogEntry
    {
        public AdvBacklogEntry(
            AdvLine line,
            string selectedChoiceId = null,
            string selectedChoiceLabel = null)
        {
            Line = line;
            SelectedChoiceId = selectedChoiceId;
            SelectedChoiceLabel = selectedChoiceLabel;
        }

        public AdvLine Line { get; }
        public string SelectedChoiceId { get; }
        public string SelectedChoiceLabel { get; }
    }
}