namespace FC.Domain.Report.Business
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Report.DAO;
    using FC.Domain.Report.Filter;
  
    public class SalesReport : BusinessLogic<SalesReportDAO>
    {
        #region Ctor

        public SalesReport()
        {
        }

        #endregion

        #region Methods

        public DataTable IndividualSalesReport(Expression<Func<IndividualSalesReportFilter, bool>> filter)
        {
            try
            {
                return DAO.IndividulaSalesReport(filter: filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SalesSummaryReport(Expression<Func<SalesSummaryReportFilter, bool>> filter)
        {
            try
            {
                return DAO.SalesSummaryReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet SalesDetailReport(Expression<Func<SalesSummaryReportFilter, bool>> summaryFilter,
            Expression<Func<IndividualSalesReportFilter, bool>> detailFilter)
        {
            try
            {
                var ds = new DataSet();
                var summary = SalesSummaryReport(filter: summaryFilter);
                var detail = IndividualSalesReport(filter: detailFilter);

                ds.Tables.Add(summary);
                ds.Tables.Add(detail);

                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable TopSalesReport(Expression<Func<TopSalesReportFilter, bool>> filter)
        {
            try
            {
                return DAO.TopSalesReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PropertyTransactionListReport(Expression<Func<PropertyTransactionListReportFilter, bool>> filter)
        {
            try
            {
                return DAO.PropertyTransactionListReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
