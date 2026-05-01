namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvCondition
    {
        public AdvCondition(
            AdvConditionKind kind,
            string key = null,
            bool expectedBool = true,
            string customKey = null)
        {
            Kind = kind;
            Key = key;
            ExpectedBool = expectedBool;
            CustomKey = customKey;
        }

        public AdvConditionKind Kind { get; }
        public string Key { get; }
        public bool ExpectedBool { get; }
        public string CustomKey { get; }

        public static AdvCondition Always()
            => new(AdvConditionKind.Always);
    }
}