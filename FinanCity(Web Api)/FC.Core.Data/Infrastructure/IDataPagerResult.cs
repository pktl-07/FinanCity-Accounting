namespace FC.Core.Data
{
    using System.Collections.Generic;

    public interface IDataPagerResult<T> where T : IEntity
    {
        IEnumerable<T> Data { get; set; }

        int Total { get; }
    }
}
