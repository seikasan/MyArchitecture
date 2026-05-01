namespace MyArchitecture.Core
{
    // 0
    public abstract class TryCommand :
        CommandBase,
        ITryCommand
    {
        public bool TryExecute()
        {
            return OnTryExecute();
        }

        protected abstract bool OnTryExecute();
    }

    // 1
    public abstract class TryCommand<TArg> :
        CommandBase,
        ITryCommand<TArg>
    {
        public bool TryExecute(TArg arg)
        {
            return OnTryExecute(arg);
        }

        protected abstract bool OnTryExecute(TArg arg);
    }

    // 2
    public abstract class TryCommand<TArg1, TArg2> :
        CommandBase,
        ITryCommand<TArg1, TArg2>
    {
        public bool TryExecute(TArg1 arg1, TArg2 arg2)
        {
            return OnTryExecute(arg1, arg2);
        }

        protected abstract bool OnTryExecute(TArg1 arg1, TArg2 arg2);
    }

    // 3
    public abstract class TryCommand<TArg1, TArg2, TArg3> :
        CommandBase,
        ITryCommand<TArg1, TArg2, TArg3>
    {
        public bool TryExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return OnTryExecute(arg1, arg2, arg3);
        }

        protected abstract bool OnTryExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
}
