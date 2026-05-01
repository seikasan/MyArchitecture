using System;
using System.Collections.Generic;
using System.Linq;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvScenarioSnapshot
    {
        public AdvScenarioSnapshot(
            string scenarioId,
            int instructionIndex,
            AdvPlaybackState playbackState,
            AdvLine currentLine,
            IEnumerable<AdvChoice> currentChoices,
            AdvWaitRequest currentWait,
            IEnumerable<AdvBacklogEntry> backlog,
            IEnumerable<AdvChoiceHistoryEntry> choiceHistory)
        {
            ScenarioId = scenarioId;
            InstructionIndex = instructionIndex;
            PlaybackState = playbackState;
            CurrentLine = currentLine;
            CurrentChoices = currentChoices?.ToArray() ??
                             Array.Empty<AdvChoice>();
            CurrentWait = currentWait;
            Backlog = backlog?.ToArray() ??
                      Array.Empty<AdvBacklogEntry>();
            ChoiceHistory = choiceHistory?.ToArray() ??
                            Array.Empty<AdvChoiceHistoryEntry>();
        }

        public string ScenarioId { get; }
        public int InstructionIndex { get; }
        public AdvPlaybackState PlaybackState { get; }
        public AdvLine CurrentLine { get; }
        public IReadOnlyList<AdvChoice> CurrentChoices { get; }
        public AdvWaitRequest CurrentWait { get; }
        public IReadOnlyList<AdvBacklogEntry> Backlog { get; }
        public IReadOnlyList<AdvChoiceHistoryEntry> ChoiceHistory { get; }
    }

    public sealed class AdvVariableEntry
    {
        public AdvVariableEntry(string key, AdvVariableValue value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public AdvVariableValue Value { get; }
    }

    public sealed class AdvStateSnapshot
    {
        public AdvStateSnapshot(
            IEnumerable<AdvVariableEntry> variables,
            IEnumerable<string> flags,
            IEnumerable<string> readMarkers)
        {
            Variables = variables?.ToArray() ??
                        Array.Empty<AdvVariableEntry>();
            Flags = flags?.ToArray() ?? Array.Empty<string>();
            ReadMarkers = readMarkers?.ToArray() ?? Array.Empty<string>();
        }

        public IReadOnlyList<AdvVariableEntry> Variables { get; }
        public IReadOnlyList<string> Flags { get; }
        public IReadOnlyList<string> ReadMarkers { get; }
    }

    public sealed class AdvSaveData
    {
        public AdvSaveData(
            AdvScenarioSnapshot scenario,
            AdvStateSnapshot state)
        {
            Scenario = scenario;
            State = state;
        }

        public AdvScenarioSnapshot Scenario { get; }
        public AdvStateSnapshot State { get; }
    }
}
