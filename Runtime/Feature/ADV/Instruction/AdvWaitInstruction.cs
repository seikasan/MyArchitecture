namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvWaitInstruction : IAdvInstruction
    {
        public AdvWaitInstruction(string waitKey)
        {
            WaitKey = waitKey;
        }

        public string WaitKey { get; }
    }
}