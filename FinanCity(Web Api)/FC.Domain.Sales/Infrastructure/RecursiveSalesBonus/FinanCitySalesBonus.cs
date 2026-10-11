namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Domain.Sales.DTO;
    using FC.Core;
    using FC.Domain.Sales.Business;

    /// <summary>
    /// FinancCity sales bonus.
    /// </summary>
    public class FinanCitySalesBonus
      : IRecursiveSalesBonus
    {
        private RecursiveSalesContext _context;
        public RecursiveSalesContext Context
        {
            get { return _context; }
        }

        public FinanCitySalesBonus(RecursiveSalesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Represens recursive sales team for target sales person.
        /// </summary>
        /// <returns>Sales team collection for target salesperson' s senior rank group.</returns>
        public IDictionary<RankType, SalesPersonTireDTO> GetSalesTeam()
        {
            var _output = new Dictionary<RankType, SalesPersonTireDTO>();
            _context.Ranks.Each(rank =>
            {
                var found = _context.Query(_context.Source, rank, _context.Target)
                .Where(x => x.RowNumber >= _context.Top.RowNumber)
                .LastOrDefault();

                if (found != null)
                    _output.Add((RankType)found.Rank, found);
            });

            return _output;
        }
    }
}
