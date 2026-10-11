namespace FC.Core
{
    public interface IRecursiveTreeGenerator<T> where T : IRecursiveItems
    {
        void SetRelation(T items);

        T[] Traverse(T items);

        void Clear();
    }
}
