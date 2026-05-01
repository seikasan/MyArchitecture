using System;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvInstructionHandler : IUtility
    {
        Type InstructionType { get; }

        AdvInstructionResult Execute(
            IAdvInstruction instruction,
            AdvInstructionContext context);
    }

    public interface IAdvInstructionHandler<in TInstruction> :
        IAdvInstructionHandler
        where TInstruction : IAdvInstruction
    {
        AdvInstructionResult Execute(
            TInstruction instruction,
            AdvInstructionContext context);
    }

    public abstract class AdvInstructionHandler<TInstruction> :
        Utility,
        IAdvInstructionHandler<TInstruction>
        where TInstruction : IAdvInstruction
    {
        public Type InstructionType => typeof(TInstruction);

        public abstract AdvInstructionResult Execute(
            TInstruction instruction,
            AdvInstructionContext context);

        AdvInstructionResult IAdvInstructionHandler.Execute(
            IAdvInstruction instruction,
            AdvInstructionContext context)
        {
            return Execute((TInstruction)instruction, context);
        }
    }
}