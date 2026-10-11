namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;

    public class LeadManagementContext
    {
        public LeadManagementContext() { }

        public IEnumerable<string> OriginClient { get; internal set; } 

        public IEnumerable<string> UpdatedClient { get; internal set; }

        public IEnumerable<string> InsertedClient { get; internal set; } 

        public IEnumerable<string> DeletedClient { get; internal set; }

        public IEnumerable<string> AlwaysBeenClient { get; internal set; } = new string[] { };

        public IEnumerable<string> HadTransactionClient { get; internal set; } = new string[] { };
    }
}
