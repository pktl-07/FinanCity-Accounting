namespace FC.Core.Infrastructure.Implements.DataCollector
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core;

    public class DataKeyConfigurator<T> : IDataKeyConfigurator<T> where T : class
    {
        public IList<Expression<Func<T, object>>> Keys { get; set; } = new List<Expression<Func<T, object>>>();

        public void Add(Expression<Func<T, object>> expression)
        {
            Keys.Add(expression);
        }
    }
}
