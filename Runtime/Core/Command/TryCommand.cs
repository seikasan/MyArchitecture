namespace MyArchitecture.Core
{
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
}
