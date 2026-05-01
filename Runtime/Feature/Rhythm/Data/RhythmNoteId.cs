using System;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmNoteId : IEquatable<RhythmNoteId>
    {
        public RhythmNoteId(string value)
        {
            Value = value ?? string.Empty;
        }

        public string Value { get; }

        public bool Equals(RhythmNoteId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is RhythmNoteId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? StringComparer.Ordinal.GetHashCode(Value) : 0;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}
