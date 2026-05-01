namespace MyArchitecture.Core
{
    // 0
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

    // 1
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

    // 2
    public abstract class Query<TArg1, TArg2, TResult> :
        QueryBase,
        IQuery<TArg1, TArg2, TResult>
    {
        public TResult Execute(TArg1 arg1, TArg2 arg2)
        {
            return OnExecute(arg1, arg2);
        }

        protected abstract TResult OnExecute(TArg1 arg1, TArg2 arg2);
    }

    // 3
    public abstract class Query<TArg1, TArg2, TArg3, TResult> :
        QueryBase,
        IQuery<TArg1, TArg2, TArg3, TResult>
    {
        public TResult Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return OnExecute(arg1, arg2, arg3);
        }

        protected abstract TResult OnExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
}
