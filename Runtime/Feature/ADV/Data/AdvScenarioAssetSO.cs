using System;
using System.Linq;
using UnityEngine;

namespace MyArchitecture.Feature.ADV
{
    
    public sealed class AdvScenarioAssetSO : ScriptableObject
    {
        [SerializeField] private AdvScenarioData scenario;

        public AdvScenario ToScenario()
        {
            if (scenario == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(AdvScenarioAssetSO)} has no scenario data.");
            }

            return scenario.ToScenario();
        }
    }

    [Serializable]
    public sealed class AdvScenarioData
    {
        public string scenarioId;
        public string entryLabel;
        public AdvInstructionData[] instructions;

        public AdvScenario ToScenario()
        {
            return new AdvScenario(
                scenarioId,
                entryLabel,
                (instructions ?? Array.Empty<AdvInstructionData>())
                .Select(instruction => instruction.ToInstruction()));
        }
    }

    [Serializable]
    public sealed class AdvInstructionData
    {
        public string kind;
        public string label;
        public string targetLabel;
        public string falseLabel;
        public string waitKey;
        public string signalName;
        public string payload;

        public string speakerId;
        public string speakerName;
        public string body;
        public string textKey;
        public string voiceKey;
        public string[] tags;

        public AdvChoiceData[] choices;

        public string variableKey;
        public string variableKind;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;

        public string conditionKind;
        public string conditionKey;
        public bool expectedBool = true;
        public string customConditionKey;

        public IAdvInstruction ToInstruction()
        {
            string normalizedKind = (kind ?? string.Empty).Trim().ToLowerInvariant();

            switch (normalizedKind)
            {
                case "label":
                    return new AdvLabelInstruction(label);
                case "line":
                    return new AdvLineInstruction(
                        new AdvLine(
                            speakerId,
                            speakerName,
                            body,
                            textKey,
                            voiceKey,
                            tags));
                case "choice":
                    return new AdvChoiceInstruction(
                        (choices ?? Array.Empty<AdvChoiceData>())
                        .Select(choice => choice.ToChoice()));
                case "jump":
                    return new AdvJumpInstruction(targetLabel);
                case "branch":
                    return new AdvBranchInstruction(
                        ToCondition(),
                        targetLabel,
                        falseLabel);
                case "set":
                case "setvariable":
                    return new AdvSetVariableInstruction(
                        new AdvVariableChange(
                            variableKey,
                            ToVariableValue()));
                case "wait":
                    return new AdvWaitInstruction(waitKey);
                case "signal":
                    return new AdvSignalInstruction(
                        new AdvSignal(signalName, payload));
                case "end":
                    return new AdvEndInstruction();
                default:
                    throw new InvalidOperationException(
                        $"ADV instruction kind is unknown: {kind}");
            }
        }

        private AdvCondition ToCondition()
        {
            string normalizedKind = (conditionKind ?? string.Empty)
                .Trim()
                .ToLowerInvariant();

            switch (normalizedKind)
            {
                case "":
                case "always":
                    return AdvCondition.Always();
                case "flag":
                    return new AdvCondition(
                        AdvConditionKind.Flag,
                        conditionKey);
                case "notflag":
                    return new AdvCondition(
                        AdvConditionKind.NotFlag,
                        conditionKey);
                case "bool":
                case "boolvariable":
                    return new AdvCondition(
                        AdvConditionKind.BoolVariable,
                        conditionKey,
                        expectedBool);
                case "custom":
                    return new AdvCondition(
                        AdvConditionKind.Custom,
                        customKey: customConditionKey);
                default:
                    throw new InvalidOperationException(
                        $"ADV condition kind is unknown: {conditionKind}");
            }
        }

        private AdvVariableValue ToVariableValue()
        {
            string normalizedKind = (variableKind ?? string.Empty)
                .Trim()
                .ToLowerInvariant();

            switch (normalizedKind)
            {
                case "bool":
                    return AdvVariableValue.Bool(boolValue);
                case "int":
                    return AdvVariableValue.Int(intValue);
                case "float":
                    return AdvVariableValue.Float(floatValue);
                case "string":
                case "":
                    return AdvVariableValue.String(stringValue);
                default:
                    throw new InvalidOperationException(
                        $"ADV variable kind is unknown: {variableKind}");
            }
        }
    }

    [Serializable]
    public sealed class AdvChoiceData
    {
        public string choiceId;
        public string label;
        public string textKey;
        public string targetLabel;
        public string signal;

        public string conditionKind;
        public string conditionKey;
        public bool expectedBool = true;
        public string customConditionKey;

        public AdvChoice ToChoice()
        {
            return new AdvChoice(
                choiceId,
                label,
                targetLabel,
                textKey,
                ToCondition(),
                signal);
        }

        private AdvCondition ToCondition()
        {
            string normalizedKind = (conditionKind ?? string.Empty)
                .Trim()
                .ToLowerInvariant();

            switch (normalizedKind)
            {
                case "":
                case "always":
                    return AdvCondition.Always();
                case "flag":
                    return new AdvCondition(
                        AdvConditionKind.Flag,
                        conditionKey);
                case "notflag":
                    return new AdvCondition(
                        AdvConditionKind.NotFlag,
                        conditionKey);
                case "bool":
                case "boolvariable":
                    return new AdvCondition(
                        AdvConditionKind.BoolVariable,
                        conditionKey,
                        expectedBool);
                case "custom":
                    return new AdvCondition(
                        AdvConditionKind.Custom,
                        customKey: customConditionKey);
                default:
                    throw new InvalidOperationException(
                        $"ADV condition kind is unknown: {conditionKind}");
            }
        }
    }
}