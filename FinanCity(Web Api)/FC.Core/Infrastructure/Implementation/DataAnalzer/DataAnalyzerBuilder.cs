namespace FC.Core.Infrastructure.Implementation
{
    using System;

    public class DataAnalyzerBuilder<T> : IDataAnalyzerBuilder<T> where T : class
    {
        private IDataAnalyzer<T> _analyser;

        public DataAnalyzerContext<T> CreateContext()
        {
            return _analyser.CreateContext();
        }

        public IDataAnalyzer<T> GetBuilder(Action<DataAnalyzerConfigurator<T>> config)
        {
            var configurator = new DataAnalyzerConfigurator<T>(); //configuration class.

            config(configurator); // trigger events.

            return _analyser = new DataAnalyzser<T>(configurator);
        }
    }
}
