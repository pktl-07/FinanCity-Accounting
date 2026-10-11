namespace FC.Api.AccountAreas.Areas.Accounts.Models
{
    using FC.Domain.Accounts.Entity;
    using System.Collections.Generic;
    
    public class CashbookTransactionModel
    {
        public IEnumerable<CashBookTransactionEntity> insertedItems { get; set; }
        public IEnumerable<CashBookTransactionEntity> updatedItems { get; set; }
        public IEnumerable<CashBookTransactionEntity> deletedItems { get; set; }
    }
}