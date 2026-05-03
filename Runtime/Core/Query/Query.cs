namespace MyArchitecture.Core
{
    public abstract class Query<TResult> :
        QueryBase,
        IQuery<TResult>
    {
        public TResult Execute()
        {
            return OnExecute();
        }

        protected abstract TResult OnExecute();
    }

    public abstract class Query<TArg, TResult> :
        QueryBase,
        IQuery<TArg, TResult>
    {
        public TResult Execute(TArg arg)
        {
            return OnExecute(arg);
        }

        protected abstract TResult OnExecute(TArg arg);
    }
}
