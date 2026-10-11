namespace FC.Api.Models
{
    using FC.Domain.Sales.Entity;
    using System.Collections.Generic;

    public class PropertyTransactionModel
    {
        public PropertyTransactionEntity PropertyTransaction { get; set; }
        public ContactEntity Contact { get; set; }
        public IEnumerable<PTOClientEntity> Clients { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> Internals { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> Externals { get; set; }
        public TNC_Entity TNC { get; set; }
        public IEnumerable<MarketingExpenseEntity> PromotionExpense { get; set; }
    }
}