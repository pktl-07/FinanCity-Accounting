namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Infrastructure.Interfaces;

    /// <summary>
    /// To build certains entities for updating, deleting and inserting result which usually come and compare with external source 
    /// </summary>
    public interface IEntityAnalyzerBuilder
    {
        /// <summary>
        /// To build entity analyzer result depending on keys projector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">Projectors</param>
        /// <param name="origin">Origin source. Which should be usually come from database</param>
        /// <param name="compare">Compare source. Which should be usually come from non database source and usually come from 
        /// user input source such as datagridview or custom arrays</param>
        /// <returns></returns>
        IEntityAnalyzer<T> Build<T>(Action<IDataKeyConfigurator<T>> keys, IEnumerable<T> origin, IEnumerable<T> compare) where T : class;
    }
}
