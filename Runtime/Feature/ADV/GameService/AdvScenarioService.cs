using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvScenarioService : GameService
    {
        private readonly AdvScenarioModel _scenarioModel;
        private readonly AdvStateModel _stateModel;
        private readonly IReadOnlyList<IAdvScenarioProvider> _scenarioProviders;
        private readonly IAdvConditionEvaluatorRegistry _conditionEvaluators;
        private readonly IAdvInstructionHandlerRegistry _instructionHandlers;
        private readonly IAdvTextFormatter _textFormatter;

        public AdvScenarioService(
            AdvScenarioModel scenarioModel,
            AdvStateModel stateModel,
            IEnumerable<IAdvScenarioProvider> scenarioProviders,
            IAdvConditionEvaluatorRegistry conditionEvaluators,
            IAdvInstructionHandlerRegistry instructionHandlers,
            IEnumerable<IAdvTextFormatter> textFormatters)
        {
            _scenarioModel = scenarioModel;
            _stateModel = stateModel;
            _scenarioProviders = scenarioProviders?.ToArray() ??
                                 Array.Empty<IAdvScenarioProvider>();
            _conditionEvaluators = conditionEvaluators;
            _instructionHandlers = instructionHandlers;
            _textFormatter = SelectTextFormatter(textFormatters);
        }

        public async UniTask StartAsync(
            string scenarioId,
            CancellationToken cancellationToken)
        {
            AdvScenario scenario = await LoadScenarioAsync(
                scenarioId,
                cancellationToken);

            Start(scenario);
        }

        public void Start(AdvScenario scenario)
        {
            int entryIndex = scenario.GetEntryIndex();

            _scenarioModel.StartScenario(scenario, entryIndex);

            this.PublishEvent(
                new AdvScenarioStartedEvent(scenario.ScenarioId));

            RunUntilPause();
        }

        public void Advance()
        {
            if (_scenarioModel.PlaybackState != AdvPlaybackState.WaitingForAdvance)
            {
                return;
            }

            _scenarioModel.SetPlaybackState(AdvPlaybackState.Running);

            RunUntilPause();
        }

        public void Choose(string choiceId)
        {
            if (_scenarioModel.PlaybackState != AdvPlaybackState.WaitingForChoice)
            {
                return;
            }

            var choice = _scenarioModel.CurrentChoices
                .FirstOrDefault(item => item.ChoiceId == choiceId);

            if (choice == null)
            {
                throw new KeyNotFoundException(
                    $"ADV choice is not found: {choiceId}");
            }

            _scenarioModel.AddChoiceHistory(choice);
            _stateModel.SetFlag($"choice:{choice.ChoiceId}");
            _scenarioModel.ClearCurrentChoices();

            this.PublishEvent(
                new AdvChoicesChangedEvent(Array.Empty<AdvChoice>()));

            if (!string.IsNullOrWhiteSpace(choice.Signal))
            {
                this.PublishEvent(
                    new AdvSignalEvent(
                        new AdvSignal(choice.Signal, choice.ChoiceId)));
            }

            if (!string.IsNullOrWhiteSpace(choice.TargetLabel))
            {
                JumpToLabel(choice.TargetLabel);
            }

            _scenarioModel.SetPlaybackState(AdvPlaybackState.Running);

            RunUntilPause();
        }

        public void Jump(string label)
        {
            JumpToLabel(label);
            _scenarioModel.SetPlaybackState(AdvPlaybackState.Running);
            RunUntilPause();
        }

        public void ResumeWait()
        {
            if (_scenarioModel.PlaybackState != AdvPlaybackState.Waiting)
            {
                return;
            }

            _scenarioModel.SetPlaybackState(AdvPlaybackState.Running);
            RunUntilPause();
        }

        public async UniTask RestoreAsync(
            AdvSaveData saveData,
            CancellationToken cancellationToken)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            AdvScenario scenario = await LoadScenarioAsync(
                saveData.Scenario.ScenarioId,
                cancellationToken);

            _stateModel.RestoreSnapshot(saveData.State);
            _scenarioModel.RestoreSnapshot(scenario, saveData.Scenario);

            this.PublishEvent(new AdvLoadCompletedEvent(saveData));
        }

        public bool CanAdvance()
        {
            return _scenarioModel.PlaybackState ==
                   AdvPlaybackState.WaitingForAdvance;
        }

        private void RunUntilPause()
        {
            AdvScenario scenario = _scenarioModel.CurrentScenario;

            if (scenario == null)
            {
                return;
            }

            int guard = 0;

            while (_scenarioModel.InstructionIndex < scenario.Instructions.Count)
            {
                if (++guard > scenario.Instructions.Count + 1024)
                {
                    throw new InvalidOperationException(
                        "ADV scenario execution looks endless.");
                }

                IAdvInstruction instruction =
                    scenario.Instructions[_scenarioModel.InstructionIndex];

                if (instruction is AdvLabelInstruction)
                {
                    MoveNext();
                    continue;
                }

                if (instruction is AdvLineInstruction lineInstruction)
                {
                    ShowLine(lineInstruction.Line);
                    return;
                }

                if (instruction is AdvChoiceInstruction choiceInstruction)
                {
                    if (ShowChoices(choiceInstruction.Choices))
                    {
                        return;
                    }

                    continue;
                }

                if (instruction is AdvJumpInstruction jumpInstruction)
                {
                    JumpToLabel(jumpInstruction.TargetLabel);
                    continue;
                }

                if (instruction is AdvBranchInstruction branchInstruction)
                {
                    Branch(branchInstruction);
                    continue;
                }

                if (instruction is AdvSetVariableInstruction setVariableInstruction)
                {
                    _stateModel.Apply(setVariableInstruction.Change);
                    MoveNext();
                    continue;
                }

                if (instruction is AdvWaitInstruction waitInstruction)
                {
                    Wait(waitInstruction.WaitKey);
                    return;
                }

                if (instruction is AdvSignalInstruction signalInstruction)
                {
                    this.PublishEvent(
                        new AdvSignalEvent(signalInstruction.Signal));
                    MoveNext();
                    continue;
                }

                if (instruction is AdvEndInstruction)
                {
                    EndScenario();
                    return;
                }

                if (_instructionHandlers.TryExecute(
                        instruction,
                        new AdvInstructionContext(
                            _scenarioModel,
                            _stateModel,
                            EventPublisher),
                        out var result))
                {
                    ApplyInstructionResult(result);
                    if (result.Flow != AdvInstructionFlow.Continue &&
                        result.Flow != AdvInstructionFlow.Jump)
                    {
                        return;
                    }

                    continue;
                }

                throw new InvalidOperationException(
                    $"ADV instruction is not handled: {instruction.GetType().FullName}");
            }

            EndScenario();
        }

        private async UniTask<AdvScenario> LoadScenarioAsync(
            string scenarioId,
            CancellationToken cancellationToken)
        {
            foreach (var provider in _scenarioProviders)
            {
                if (!provider.CanProvide(scenarioId))
                {
                    continue;
                }

                return await provider.LoadAsync(
                    scenarioId,
                    cancellationToken);
            }

            throw new InvalidOperationException(
                $"ADV scenario provider is not found: {scenarioId}");
        }

        private void ShowLine(AdvLine line)
        {
            AdvLine formattedLine = _textFormatter.FormatLine(
                line,
                _stateModel.CreateSnapshot());

            _scenarioModel.SetCurrentLine(formattedLine);
            _scenarioModel.AddBacklog(new AdvBacklogEntry(formattedLine));
            _scenarioModel.SetPlaybackState(AdvPlaybackState.WaitingForAdvance);
            MoveNext();

            if (!string.IsNullOrWhiteSpace(formattedLine.TextKey))
            {
                _stateModel.MarkRead(formattedLine.TextKey);
            }

            this.PublishEvent(new AdvLineChangedEvent(formattedLine));
        }

        private bool ShowChoices(IReadOnlyList<AdvChoice> choices)
        {
            var availableChoices = choices
                .Where(choice => IsConditionMatched(choice.Condition))
                .ToArray();

            if (availableChoices.Length == 0)
            {
                MoveNext();
                return false;
            }

            _scenarioModel.SetCurrentChoices(availableChoices);
            _scenarioModel.SetPlaybackState(AdvPlaybackState.WaitingForChoice);
            MoveNext();

            this.PublishEvent(
                new AdvChoicesChangedEvent(availableChoices));

            return true;
        }

        private void Branch(AdvBranchInstruction instruction)
        {
            string targetLabel = IsConditionMatched(instruction.Condition)
                ? instruction.TrueLabel
                : instruction.FalseLabel;

            if (string.IsNullOrWhiteSpace(targetLabel))
            {
                MoveNext();
                return;
            }

            JumpToLabel(targetLabel);
        }

        private bool IsConditionMatched(AdvCondition condition)
        {
            if (condition == null ||
                condition.Kind == AdvConditionKind.Always)
            {
                return true;
            }

            switch (condition.Kind)
            {
                case AdvConditionKind.Flag:
                    return _stateModel.HasFlag(condition.Key);
                case AdvConditionKind.NotFlag:
                    return !_stateModel.HasFlag(condition.Key);
                case AdvConditionKind.BoolVariable:
                    return _stateModel.TryGetVariable(
                               condition.Key,
                               out var value) &&
                           value.Kind == AdvVariableKind.Bool &&
                           value.BoolValue == condition.ExpectedBool;
                case AdvConditionKind.Custom:
                    return _conditionEvaluators.Evaluate(
                        condition.CustomKey,
                        _stateModel.CreateSnapshot());
                default:
                    return false;
            }
        }

        private void Wait(string waitKey)
        {
            var wait = new AdvWaitRequest(waitKey);

            _scenarioModel.SetCurrentWait(wait);
            _scenarioModel.SetPlaybackState(AdvPlaybackState.Waiting);
            MoveNext();

            this.PublishEvent(new AdvWaitStartedEvent(wait));
        }

        private void ApplyInstructionResult(AdvInstructionResult result)
        {
            switch (result.Flow)
            {
                case AdvInstructionFlow.Continue:
                    MoveNext();
                    break;
                case AdvInstructionFlow.Pause:
                    _scenarioModel.SetPlaybackState(AdvPlaybackState.Waiting);
                    break;
                case AdvInstructionFlow.Jump:
                    JumpToLabel(result.TargetLabel);
                    break;
                case AdvInstructionFlow.End:
                    EndScenario();
                    break;
                default:
                    MoveNext();
                    break;
            }
        }

        private void JumpToLabel(string label)
        {
            _scenarioModel.SetInstructionIndex(
                _scenarioModel.CurrentScenario.FindLabelIndex(label));
        }

        private void MoveNext()
        {
            _scenarioModel.SetInstructionIndex(
                _scenarioModel.InstructionIndex + 1);
        }

        private void EndScenario()
        {
            string scenarioId = _scenarioModel.CurrentScenarioId;

            _scenarioModel.EndScenario();

            this.PublishEvent(
                new AdvScenarioEndedEvent(scenarioId));
        }

        private static IAdvTextFormatter SelectTextFormatter(
            IEnumerable<IAdvTextFormatter> textFormatters)
        {
            IAdvTextFormatter defaultFormatter = null;
            IAdvTextFormatter customFormatter = null;

            foreach (var formatter in textFormatters ?? Array.Empty<IAdvTextFormatter>())
            {
                if (formatter is DefaultAdvTextFormatter)
                {
                    defaultFormatter ??= formatter;
                    continue;
                }

                if (customFormatter != null)
                {
                    throw new InvalidOperationException(
                        "Multiple custom ADV text formatters are registered.");
                }

                customFormatter = formatter;
            }

            return customFormatter ?? defaultFormatter ?? new DefaultAdvTextFormatter();
        }
    }
}
