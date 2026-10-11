namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Key configuration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataKeyConfigurator<T> where T : class
    {
        /// <summary>
        /// Key container
        /// </summary>
        IList<Expression<Func<T, object>>> Keys { get; }

        /// <summary>
        /// To add projector key to select specific property
        /// </summary>
        /// <param name="key"></param>
        void Add(Expression<Func<T, object>> key);
    }
}
