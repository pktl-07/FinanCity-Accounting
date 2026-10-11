namespace FC.Core.Infrastructure.Implementation
{
    using System.Collections.Generic;

    public class DataAnalyzerContext<T> where T : class
    {
        public DataAnalyzerContext()
        {
            InsertItems = new List<T>();
            UpdateItems = new List<T>();
            DeleteItems = new List<T>();
        }

        public ICollection<T> DeleteItems { get; internal set; }

        public ICollection<T> UpdateItems { get; internal set; }

        public ICollection<T> InsertItems { get; internal set; }
    }
}
