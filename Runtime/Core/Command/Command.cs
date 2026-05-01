namespace MyArchitecture.Core
{
    // 0
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

    // 1
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

    // 2
    public abstract class Command<TArg1, TArg2> :
        CommandBase,
        ICommand<TArg1, TArg2>
    {
        public void Execute(TArg1 arg1, TArg2 arg2)
        {
            OnExecute(arg1, arg2);
        }

        protected abstract void OnExecute(TArg1 arg1, TArg2 arg2);
    }

    // 3
    public abstract class Command<TArg1, TArg2, TArg3> :
        CommandBase,
        ICommand<TArg1, TArg2, TArg3>
    {
        public void Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            OnExecute(arg1, arg2, arg3);
        }

        protected abstract void OnExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
}
