namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;
    using System.Collections.Generic;

    public class CommissionSchemeModel
    {
        public CommissionStructureEntity CommissionScheme { get; set; }
        public IEnumerable<CommissionStructureDetailEntity> CommissionSchemeDetail { get; set; }
    }

    public class CommissionSchemeCrateOrUpdateModel
    {
        public CommissionStructureEntity CommissionScheme { get; set; }

        public IEnumerable<CommissionStructureDetailEntity> Inserted { get; set; }
        public IEnumerable<CommissionStructureDetailEntity> Updated { get; set; }
        public IEnumerable<CommissionStructureDetailEntity> Deleted { get; set; }
    }
}