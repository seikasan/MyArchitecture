namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvSignal
    {
        public AdvSignal(string signalName, string payload = null)
        {
            SignalName = signalName;
            Payload = payload;
        }

        public string SignalName { get; }
        public string Payload { get; }
    }
}