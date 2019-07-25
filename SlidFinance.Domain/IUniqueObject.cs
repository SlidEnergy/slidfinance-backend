namespace SlidFinance.Domain
{
    public interface IUniqueObject<T>
    {
        T Id { get; set; }
    }

    public interface IUniqueObject : IUniqueObject<int>
    {

    }
}
