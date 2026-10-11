namespace FC.Domain.Sales.DAO
{
    using System;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;

    public class MarketingExpenseDAO : DAO<MarketingExpenseEntity>
    {
        #region Methods

        public decimal GetMarketingExpense(string spsk)
        {
            try
            {
                var query = $@"SELECT ISNULL(SUM(ROUND({With<MarketingExpenseEntity>(x => x.Amount)},2)),0) FROM {Table<PropertyTransactionDetailEntity>()}
                INNER JOIN {Table<MarketingExpenseEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<MarketingExpenseEntity>(x => x.PTOSK)}
                WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)} = '{spsk}' AND 
                {With<PropertyTransactionDetailEntity>(x => x.TransType)} <> {(int)FC.Core.TransType.ExternalSalesPerson}";

                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
