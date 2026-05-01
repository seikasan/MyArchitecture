using System;
using System.Collections.Generic;
using System.Linq;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public partial class AdvStateModel :
        Model
    {
        private readonly Dictionary<string, AdvVariableValue> _variables = new();
        private readonly HashSet<string> _flags = new();
        private readonly HashSet<string> _readMarkers = new();

        public IReadOnlyDictionary<string, AdvVariableValue> Variables => _variables;
        public IReadOnlyCollection<string> Flags => _flags;
        public IReadOnlyCollection<string> ReadMarkers => _readMarkers;

        public void SetVariable(string key, AdvVariableValue value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(
                    "Variable key is null or empty.",
                    nameof(key));
            }

            _variables[key] = value;
        }

        public void Apply(AdvVariableChange change)
        {
            SetVariable(change.Key, change.Value);
        }

        public bool TryGetVariable(string key, out AdvVariableValue value)
        {
            return _variables.TryGetValue(key, out value);
        }

        public AdvVariableValue GetVariable(string key)
        {
            if (_variables.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException(
                $"ADV variable is not found: {key}");
        }

        public void SetFlag(string flag, bool enabled = true)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                return;
            }

            if (enabled)
            {
                _flags.Add(flag);
                return;
            }

            _flags.Remove(flag);
        }

        public bool HasFlag(string flag)
        {
            return !string.IsNullOrWhiteSpace(flag) &&
                   _flags.Contains(flag);
        }

        public void MarkRead(string marker)
        {
            if (!string.IsNullOrWhiteSpace(marker))
            {
                _readMarkers.Add(marker);
            }
        }

        public bool IsRead(string marker)
        {
            return !string.IsNullOrWhiteSpace(marker) &&
                   _readMarkers.Contains(marker);
        }

        public AdvStateSnapshot CreateSnapshot()
        {
            return new AdvStateSnapshot(
                _variables.Select(pair =>
                    new AdvVariableEntry(pair.Key, pair.Value)),
                _flags,
                _readMarkers);
        }

        public void RestoreSnapshot(AdvStateSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            _variables.Clear();
            _flags.Clear();
            _readMarkers.Clear();

            foreach (var variable in snapshot.Variables)
            {
                _variables[variable.Key] = variable.Value;
            }

            foreach (var flag in snapshot.Flags)
            {
                _flags.Add(flag);
            }

            foreach (var marker in snapshot.ReadMarkers)
            {
                _readMarkers.Add(marker);
            }
        }
    }
}
