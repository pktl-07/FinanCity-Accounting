namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Domain.Sales.DTO;

    public class RecursiveSalesContext
    {
        public IEnumerable<SalesPersonTireDTO> Source { get; internal set; }

        public SalesPersonTireDTO Top
        {
            get
            {
                for (int i = 0; i < Ranks.Length; i++)
                {
                    var top = Query(Source, Ranks[i], Target).LastOrDefault();
                    if (top != null)
                    {
                        return top;
                    }
                }
                return null;
            }
        }

        public SalesPersonTireDTO Target { get; internal set; }

        public int[] Ranks { get; internal set; }

        public IEnumerable<SalesPersonTireDTO> Query(IEnumerable<SalesPersonTireDTO> source,
            int rank,
            SalesPersonTireDTO target)
        {
            return source.Where(x =>
            x.Rank == rank &&
            x.IsActive == true &&
            x.Level < target.Level &&
            x.GroupNum == target.GroupNum &&
            x.RowNumber < target.RowNumber);
        }
    }
}
