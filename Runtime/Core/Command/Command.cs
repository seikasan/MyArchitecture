namespace MyArchitecture.Core
{
    public abstract class Command :
        CommandBase,
        ICommand
    {
        public void Execute()
        {
            OnExecute();
        }

        protected abstract void OnExecute();
    }

    public abstract class Command<TArg> :
        CommandBase,
        ICommand<TArg>
    {
        public void Execute(TArg arg)
        {
            OnExecute(arg);
        }

        protected abstract void OnExecute(TArg arg);
    }
}
