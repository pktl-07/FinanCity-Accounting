namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;

    public class ReferralSalesOrderModel
    {
        public PropertyTransactionEntity PropertyTransaction { get; set; }

        public ContactEntity Contact { get; set; }

        public TNC_Entity TNC { get; set; }

        public PropertyTransactionDetailEntity Internal { get; set; }

        public PTOClientEntity Client { get; set; }
    }
}