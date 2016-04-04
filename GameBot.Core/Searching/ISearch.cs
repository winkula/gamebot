namespace GameBot.Core.Searching
{
    public interface ISearch<T>
    {
        T Search(T node);
    }
}
