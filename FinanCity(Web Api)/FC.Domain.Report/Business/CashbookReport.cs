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

    public class CashbookReport : BusinessLogic<CashbookReportDAO>
    {
        #region Fields

        private AccountingReport _accountingReport;
        private CashbookSetup _cashbookSetup;
        private BTable _generalLedger;

        #endregion

        #region Ctor

        public CashbookReport(AccountingReport accountingReport,
            BTable generalLedger,
            CashbookSetup cashbookSetup)
        {
            _cashbookSetup = cashbookSetup;
            _accountingReport = accountingReport;
            _generalLedger = generalLedger;
        }

        #endregion

        #region Methods

        public DataTable CashbookLogReport(Expression<Func<CashbookLogReportFilter, bool>> filter,
            out decimal openingBalance)
        {
            try
            {
                openingBalance = OpengingBalance(filter: filter);

                var a = openingBalance.CloneJson();
                var dt = DAO.CashbookLogReport(filter: filter);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var debit = (decimal)dt.Rows[i]["Debit"];
                    var credit = (decimal)dt.Rows[i]["Credit"];

                    a = a + (debit - credit); // update opening balance
                    dt.Rows[i]["ClosingAmount"] = a;
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CashbookReceiptReport(Expression<Func<CashbookReceiptPaymentReportFilter,bool>> filter)
        {
            try
            {
                filter = filter.Erase($"{nameof(CashbookReceiptPaymentReportFilter.TransType)}");
                filter = filter.Constant((f) => f.TransType == (int)TransType.CashBookReceived);
                return DAO.CashbookReceiptPaymentReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CashbookPaymentReport(Expression<Func<CashbookReceiptPaymentReportFilter, bool>> filter)
        {
            try
            {
                filter = filter.Erase("TransType");
                filter = filter.Constant((f) => f.TransType == (int)TransType.CashBookPayment);
                return DAO.CashbookReceiptPaymentReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private decimal OpengingBalance(Expression<Func<CashbookLogReportFilter, bool>> filter)
        {
            try
            {
                var accsk = "";
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CashbookLogReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                if (!string.IsNullOrWhiteSpace(a.CBSK))
                    accsk = _cashbookSetup.Finds(filter: f => f.SysKey == a.CBSK).Single().AccSK;

                var opening = _generalLedger.GetOpeningBalance(date: fromDate.Value, accsk: accsk);
                return opening;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion
    }
}
