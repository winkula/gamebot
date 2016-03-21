namespace GameBot.Core.Searching
{
    public interface ISearch<TNode>
    {
        TNode Search(TNode node);
    }
}
