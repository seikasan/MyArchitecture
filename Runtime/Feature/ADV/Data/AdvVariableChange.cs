namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvVariableChange
    {
        public AdvVariableChange(string key, AdvVariableValue value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public AdvVariableValue Value { get; }
    }
}