namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class CurrencyRate : BusinessLogic<CurrencyRateEntity, CurrencyRateDAO>
    {
        #region Methods

        public IEnumerable<CurrencyRateDTO> GetCurrencyRateDTO(Expression<Func<CurrencyRateDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetCurrencyRateDTO(filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<CurrencyRateDTO> GetPager(int currentPage, int pageSize, Expression<Func<CurrencyRateDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
