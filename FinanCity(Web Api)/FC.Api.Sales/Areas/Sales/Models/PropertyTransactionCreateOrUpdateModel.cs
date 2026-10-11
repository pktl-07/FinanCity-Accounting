namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using System.Collections.Generic;
    using FC.Domain.Sales.Entity;
    
    public class PropertyTransactionCreateOrUpdateModel
    {
        public PropertyTransactionEntity Property { get; set; }
        public ContactEntity Contact { get; set; }
        public TNC_Entity TNC { get; set; }

        public IEnumerable<PTOClientEntity> ClientInsertedItems { get; set; }
        public IEnumerable<PTOClientEntity> ClientUpdatedItems { get; set; }
        public IEnumerable<PTOClientEntity> ClientDeletedItems { get; set; }

        public IEnumerable<PropertyTransactionDetailEntity> InternalInsertedItems { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> InternalUpdatedItems { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> InternalDeletedItems { get; set; }

        public IEnumerable<PropertyTransactionDetailEntity> ExternalInsertedItems { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> ExternalUpdatedItems { get; set; }
        public IEnumerable<PropertyTransactionDetailEntity> ExternalDeletedItems { get; set; }

        public IEnumerable<MarketingExpenseEntity> PromotionItemExpenseInsertedItems { get; set; }
        public IEnumerable<MarketingExpenseEntity> PromotionItemExpenseUpdatedItems { get; set; }
        public IEnumerable<MarketingExpenseEntity> PromotionItemExpenseDeletedITems { get; set; }
    }
}