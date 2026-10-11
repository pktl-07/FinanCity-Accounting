namespace FC.Core.Infrastructure
{
    internal interface IExceptor<T>
    {
        T[] GetExceptResult();
    }
}
