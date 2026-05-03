namespace MyArchitecture.Core
{
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
}