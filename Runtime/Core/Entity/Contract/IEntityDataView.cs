namespace MyArchitecture.Core
{
    public interface IEntityDataView<in TData>
    {
        void Apply(TData data);
    }
}