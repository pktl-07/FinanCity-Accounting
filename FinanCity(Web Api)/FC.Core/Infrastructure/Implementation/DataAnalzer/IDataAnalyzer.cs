namespace FC.Core.Infrastructure.Implementation
{
    public interface IDataAnalyzer<T> where T : class
    {
        DataAnalyzerContext<T> CreateContext();
    }
}
