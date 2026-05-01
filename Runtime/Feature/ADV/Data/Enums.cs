namespace MyArchitecture.Feature.ADV
{
    public enum AdvPlaybackState
    {
        Stopped,
        Running,
        WaitingForAdvance,
        WaitingForChoice,
        Waiting,
        Ended
    }

    public enum AdvVariableKind
    {
        Bool,
        Int,
        Float,
        String
    }

    public enum AdvConditionKind
    {
        Always,
        Flag,
        NotFlag,
        BoolVariable,
        Custom
    }
}