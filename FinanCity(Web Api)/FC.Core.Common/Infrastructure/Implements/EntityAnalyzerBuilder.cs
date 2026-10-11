namespace FC.Core.Infrastructure.Implements.DataCollector
{
    using System;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Infrastructure.Interfaces;

    public class EntityAnalyzerBuilder : IEntityAnalyzerBuilder
    {
        public IEntityAnalyzer<T> Build<T>(Action<IDataKeyConfigurator<T>> keys,
            IEnumerable<T> origin,
            IEnumerable<T> compare) where T : class
        {
            return new EntityAnalyzer<T>(keys, origin, compare);
        }
    }
}
