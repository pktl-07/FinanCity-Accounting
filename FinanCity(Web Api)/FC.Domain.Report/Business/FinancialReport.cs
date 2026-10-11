namespace FC.Domain.Report.Business
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Report.DAO;
    using FC.Domain.Report.Filter;

    public class FinancialReport : BusinessLogic<FinancialReportDAO>
    {
        #region Fields

        private SalesReport _salesReport;
        private OpeningAndClosingBalanceCollection<CommissionReceivedReportDTO> _openClose;
        private CashbookSetup _cashbookSetup;

        #endregion

        #region Ctor

        public FinancialReport(SalesReport salesReport,
           CashbookSetup cashbookSetup,
           OpeningAndClosingBalanceCollection<CommissionReceivedReportDTO> openClose)
        {
            _salesReport = salesReport;
            _openClose = openClose;
            _cashbookSetup = cashbookSetup;
        }

        #endregion

        #region Methods

        public DataSet CommissionReceivableReport(Expression<Func<CommissionReceivableReportFilter, bool>> filter)
        {
            try
            {
                int? fromdate = null;
                int? todate = null;
                var ds = new DataSet();
                var s = filter.To<CommissionReceivableReportFilter, SalesSummaryReportFilter>();

                s = s.Constant((x) => x.ReceivedStatus == (int)SalesViewType.Pending);
                s.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionReceivableReportFilter.SalesDate)}")
                    {
                        if (fromdate == null)
                            fromdate = int.Parse(value);
                        else if (todate == null)
                            todate = int.Parse(value);
                    }
                });
                s = s.Erase($"{nameof(CommissionReceivableReportFilter.SalesDate)}");
                s = s.Constant(x => x.SalesDate <= todate);

                var salesSummary = _salesReport.SalesSummaryReport(filter: s);
                var receivable = DAO.CommissionReceivableReport(filter: filter);

                ds.Tables.Add(salesSummary);
                ds.Tables.Add(receivable);

                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CommissionReceivedReport(Expression<Func<CommissionReceivedReportFilter, bool>> filter)
        {
            try
            {
                var cashbooks = _cashbookSetup.Finds().ToList();
              
                var reportDTO = DAO.CommissionReceivedReport(filter: filter)
                    .OrderBy(x => x.InvoiceNo)
                    .ThenBy(x => x.Inv_VoucherId)
                    .ThenBy(x => x.Auto)
                    .ToList();

                reportDTO = _openClose.Intialize(reportDTO,
                   x => x.InvoiceAmt,
                   x => x.ReceivedAmt,
                   x => x.Balance).ToLookUp(x => x.InvoiceSK)
                   .GetResult()
                   .OrderBy(x => x.InvoiceNo).ToList();

                for (int i = 0; i < reportDTO.Count; i++)
                {
                    var cashbookEntity = cashbooks.Where(x => x.SysKey == reportDTO[i].CBSK).SingleOrDefault();

                    if (cashbookEntity != null)
                        reportDTO[i].Cashbook = cashbookEntity.CashbookName;

                    reportDTO[i].ReceiptDate = GeneralUtility.ConvertDisplayDateFormat(reportDTO[i].ReceiptDate);
                }

                return IEnumerableExtension.ToDataTable(reportDTO, (string)"CommissionReceivedReport");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PayableHeaderDataSet(Expression<Func<CommissionPayableReportFilter, bool>> filter)
        {
            try
            {
                return DAO.PayableHeaderDataSet(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PayableDetailDataSet(Expression<Func<CommissionPayableReportFilter, bool>> filter)
        {
            try
            {
                return DAO.PayableDetailDataSet(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal OpeningPayableBalance(int startDate)
        {
            try
            {
                return DAO.OpeningPayableBalance(startDate: startDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         
        public DataTable CommissionPayableBSReport(Expression<Func<CommissionPayableBSReportFilter, bool>> filter)
        {
            try
            {
                return DAO.CommissionPayableBSReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    #endregion
}