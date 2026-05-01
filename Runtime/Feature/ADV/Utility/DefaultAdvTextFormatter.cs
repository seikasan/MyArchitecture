using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvTextFormatter : IUtility
    {
        AdvLine FormatLine(
            AdvLine line,
            AdvStateSnapshot state);
    }

    public sealed class DefaultAdvTextFormatter :
        Utility,
        IAdvTextFormatter
    {
        public AdvLine FormatLine(
            AdvLine line,
            AdvStateSnapshot state)
        {
            if (line == null)
            {
                return null;
            }

            return new AdvLine(
                line.SpeakerId,
                FormatText(line.SpeakerName, state),
                FormatText(line.Body, state),
                line.TextKey,
                line.VoiceKey,
                line.Tags);
        }

        private static string FormatText(
            string text,
            AdvStateSnapshot state)
        {
            if (string.IsNullOrEmpty(text) ||
                state == null)
            {
                return text;
            }

            string result = text;

            foreach (var variable in state.Variables)
            {
                result = result.Replace(
                    "{" + variable.Key + "}",
                    variable.Value?.ToString() ?? string.Empty);
            }

            return result;
        }
    }
}