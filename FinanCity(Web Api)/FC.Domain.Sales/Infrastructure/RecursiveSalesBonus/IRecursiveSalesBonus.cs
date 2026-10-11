namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Sales.DTO;

    public interface IRecursiveSalesBonus
    {
        /// <summary>
        /// Recursive sales context.
        /// </summary>
        RecursiveSalesContext Context { get; }

        /// <summary>
        /// Recursive sales team.
        /// </summary>
        /// <returns></returns>
        IDictionary<RankType, SalesPersonTireDTO> GetSalesTeam();
    }
}
