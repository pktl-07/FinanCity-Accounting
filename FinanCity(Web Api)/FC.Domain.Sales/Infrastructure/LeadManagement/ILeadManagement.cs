namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;

    public interface ILeadManagement
    {
        LeadManagementContext Context { get; }

        IEnumerable<string> DePromoteLeadCollection { get; }

        IEnumerable<string> PromoteClientCollection { get; }
    }
}
