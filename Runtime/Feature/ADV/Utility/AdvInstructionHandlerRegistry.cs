using System;
using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvInstructionHandlerRegistry : IUtility
    {
        void Register(IAdvInstructionHandler handler);

        void Register<TInstruction>(
            IAdvInstructionHandler<TInstruction> handler)
            where TInstruction : IAdvInstruction;

        bool TryExecute(
            IAdvInstruction instruction,
            AdvInstructionContext context,
            out AdvInstructionResult result);
    }

    public sealed class AdvInstructionHandlerRegistry :
        Utility,
        IAdvInstructionHandlerRegistry
    {
        private readonly Dictionary<Type, IAdvInstructionHandler> _handlers = new();

        public AdvInstructionHandlerRegistry(
            IEnumerable<IAdvInstructionHandler> handlers = null)
        {
            foreach (var handler in handlers ?? Array.Empty<IAdvInstructionHandler>())
            {
                Register(handler);
            }
        }

        public void Register(IAdvInstructionHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Register(handler.InstructionType, handler);
        }

        public void Register<TInstruction>(
            IAdvInstructionHandler<TInstruction> handler)
            where TInstruction : IAdvInstruction
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Register(typeof(TInstruction), handler);
        }

        private void Register(
            Type instructionType,
            IAdvInstructionHandler handler)
        {
            if (instructionType == null ||
                !typeof(IAdvInstruction).IsAssignableFrom(instructionType))
            {
                throw new InvalidOperationException(
                    $"{handler.GetType().Name} has invalid instruction type.");
            }

            if (_handlers.TryGetValue(instructionType, out var existingHandler) &&
                !ReferenceEquals(existingHandler, handler))
            {
                throw new InvalidOperationException(
                    $"ADV instruction handler for {instructionType.Name} is already registered.");
            }

            _handlers[instructionType] = handler;
        }

        public bool TryExecute(
            IAdvInstruction instruction,
            AdvInstructionContext context,
            out AdvInstructionResult result)
        {
            if (instruction != null &&
                _handlers.TryGetValue(instruction.GetType(), out var handler))
            {
                result = handler.Execute(instruction, context);
                return true;
            }

            result = default;
            return false;
        }
    }
}