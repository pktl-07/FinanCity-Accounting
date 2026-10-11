namespace FC.Domain.Sales.Infrastructure
{
    using FC.Domain.Sales.Entity;
    using System.Collections.Generic;

    public class OverridingProfileContext
    {
        public PropertyTransactionEntity SalesOrder { get; internal set; }
    
        public IEnumerable<PropertyTransactionDetailEntity> SalesDetails { get; internal set; }

    }
}
