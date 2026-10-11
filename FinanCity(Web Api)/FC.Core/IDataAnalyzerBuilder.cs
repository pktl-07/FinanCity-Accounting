namespace FC.Core
{
    using System;
    using FC.Core.Infrastructure.Implementation;

    public interface IDataAnalyzerBuilder<T> where T : class
    {
        IDataAnalyzer<T> GetBuilder(Action<DataAnalyzerConfigurator<T>> config);
    }
}
