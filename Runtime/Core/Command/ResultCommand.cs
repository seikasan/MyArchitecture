namespace MyArchitecture.Core
{
    // 0
    public abstract class ResultCommand<TResult> :
        CommandBase,
        IResultCommand<TResult>
        where TResult : ICommandResult
    {
        public TResult Execute()
        {
            return OnExecute();
        }

        protected abstract TResult OnExecute();
    }

    // 1
    public abstract class ResultCommand<TArg, TResult> :
        CommandBase,
        IResultCommand<TArg, TResult>
        where TResult : ICommandResult
    {
        public TResult Execute(TArg arg)
        {
            return OnExecute(arg);
        }

        protected abstract TResult OnExecute(TArg arg);
    }

    // 2
    public abstract class ResultCommand<TArg1, TArg2, TResult> :
        CommandBase,
        IResultCommand<TArg1, TArg2, TResult>
        where TResult : ICommandResult
    {
        public TResult Execute(TArg1 arg1, TArg2 arg2)
        {
            return OnExecute(arg1, arg2);
        }

        protected abstract TResult OnExecute(TArg1 arg1, TArg2 arg2);
    }

    // 3
    public abstract class ResultCommand<TArg1, TArg2, TArg3, TResult> :
        CommandBase,
        IResultCommand<TArg1, TArg2, TArg3, TResult>
        where TResult : ICommandResult
    {
        public TResult Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return OnExecute(arg1, arg2, arg3);
        }

        protected abstract TResult OnExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
}