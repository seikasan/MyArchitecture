namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvVariableValue
    {
        private AdvVariableValue(
            AdvVariableKind kind,
            bool boolValue,
            int intValue,
            float floatValue,
            string stringValue)
        {
            Kind = kind;
            BoolValue = boolValue;
            IntValue = intValue;
            FloatValue = floatValue;
            StringValue = stringValue;
        }

        public AdvVariableKind Kind { get; }
        public bool BoolValue { get; }
        public int IntValue { get; }
        public float FloatValue { get; }
        public string StringValue { get; }

        public static AdvVariableValue Bool(bool value)
            => new(AdvVariableKind.Bool, value, 0, 0f, null);

        public static AdvVariableValue Int(int value)
            => new(AdvVariableKind.Int, false, value, 0f, null);

        public static AdvVariableValue Float(float value)
            => new(AdvVariableKind.Float, false, 0, value, null);

        public static AdvVariableValue String(string value)
            => new(AdvVariableKind.String, false, 0, 0f, value);

        public override string ToString()
        {
            return Kind switch
            {
                AdvVariableKind.Bool => BoolValue.ToString(),
                AdvVariableKind.Int => IntValue.ToString(),
                AdvVariableKind.Float => FloatValue.ToString(),
                AdvVariableKind.String => StringValue ?? string.Empty,
                _ => string.Empty
            };
        }
    }
}