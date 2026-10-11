namespace FC.Domain.Sales.Business
{
    using System;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class MarketingExpense : BusinessLogic<MarketingExpenseEntity, MarketingExpenseDAO>
    {
        #region Methods
       
        public decimal GetMarketingExpense(string spsk)
        {
            try
            {
                return DAO.GetMarketingExpense(spsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
