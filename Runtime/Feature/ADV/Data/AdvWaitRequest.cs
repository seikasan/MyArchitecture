namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvWaitRequest
    {
        public AdvWaitRequest(string waitKey)
        {
            WaitKey = waitKey;
        }

        public string WaitKey { get; }
    }
}