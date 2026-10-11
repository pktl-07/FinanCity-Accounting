namespace FC.Domain.Report.Business
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Report.DAO;
    using FC.Domain.Report.Filter;

    public class AgedReceivables : BusinessLogic<AgedReceivablesDAO>
    {
        #region Ctor

        public AgedReceivables()
        {
        }

        #endregion

        #region Methods

        public DataTable AgedReceivableSummaryReport(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            try
            {
                return DAO.AgedReceivableSummaryReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable AgedReceivableDetailReport(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            try
            {
                return DAO.AgedReceivableDetailReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
