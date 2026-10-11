namespace FC.Core.Infrastructure.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class DataAnalyzerConfigurator<T> where T : class
    {
        public IEnumerable<T> PostData { get; set; }

        public IEnumerable<T> OriginData { get; set; }

        public Expression<Func<T, object>> KeySelector { get; set; }

        public Action<T> BeforeInsert { get; set; }
    }
}
