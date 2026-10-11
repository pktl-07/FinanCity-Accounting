namespace FC.Core.Infrastructure.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Custom IEqualityComparer for accepting with projector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityAnalyzer<T> : IEqualityComparer<T> where T : class
    {
        /// <summary>
        /// To get updated source
        /// </summary>
        /// <param name="distinct">When true which effect build in Linq's result. The result of outcome will be distinct according to the key selector</param>
        /// <returns>Updated Source</returns>
        IEnumerable<T> UpdatedSource(bool distinct = false);

        /// <summary>
        /// To get deleted source
        /// </summary>
        /// <param name="distinct">When true which effect build in Linq's result. The result of outcome will be distinct according to the key selector</param>
        /// <returns>Deleted Source</returns>
        IEnumerable<T> DeletedSource(bool distinct = false);

        /// <summary>
        /// To get inserted source
        /// </summary>
        /// <param name="distinct">When true which effect build in Linq's result. The result of outcome will be distinct according to the key selector</param>
        /// <returns>Inserted Source</returns>
        IEnumerable<T> InsertedSource(bool distinct = false);
    }
}
