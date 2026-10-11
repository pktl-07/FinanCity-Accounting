namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class CurrencyRateDAO : DAO<CurrencyRateEntity>
    {
        #region Methods

        public IDataPagerResult<CurrencyRateDTO> GetPager(int currentPage, int pageSize, Expression<Func<CurrencyRateDTO, bool>> filter = null)
        {
            try
            {
                return ExcutePagerResult<CurrencyRateDTO>(CurrencyRateQuery(filter: filter), currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<CurrencyRateDTO> GetCurrencyRateDTO(Expression<Func<CurrencyRateDTO, bool>> filter = null)
        {
            try
            {
                return DbExcutor<CurrencyRateDTO>.ExcuteReader(CurrencyRateQuery(filter: filter));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Utilities

        private string CurrencyRateQuery(Expression<Func<CurrencyRateDTO, bool>> filter = null)
        {
            return $@"SELECT
                    { With<CurrencyEntity>(x => x.SysKey)} [{nameof(CurrencyRateDTO.SysKey)}],
                    { With<CurrencyEntity>(x => x.Country) } [{nameof(CurrencyRateDTO.Country)}],
                    { With<CurrencyEntity>(x => x.Currency_AlphaCode) } [{nameof(CurrencyRateDTO.CurrencyCode)}],
                    { With<CurrencyRateEntity>(x => x.Description) } [{nameof(CurrencyRateDTO.Description)}],
                    { With<CurrencyRateEntity>(x => x.Symbol) } [{nameof(CurrencyRateDTO.Symbol)}],
                    { With<CurrencyRateEntity>(x => x.Operator) } [{nameof(CurrencyRateDTO.Operator)}],
                    { With<CurrencyRateEntity>(x => x.Rate)} [{nameof(CurrencyRateDTO.Rate)}]
                    FROM {Table<CurrencyEntity>()}
                    INNER JOIN {Table<CurrencyRateEntity>()} ON {With<CurrencyEntity>(x => x.SysKey)} = {With<CurrencyRateEntity>(x => x.SysKey)}
                    WHERE {filter.ToSQL()} ORDER BY {With<CurrencyRateEntity>(x => x.Auto)} DESC";
        }

        #endregion
    }
}
