namespace SlidFinance.Domain
{
    public interface IUniqueObject<T>
    {
        T Id { get; }
    }

    public interface IUniqueObject : IUniqueObject<int>
    {

    }
}
