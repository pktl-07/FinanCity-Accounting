namespace FC.Domain.Report.DAO
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Report.Filter;
  
    public class CashbookReportDAO : DAO
    {
        #region Methods

        public DataTable CashbookLogReport(Expression<Func<CashbookLogReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(TaxReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);
                    }
                });

                var cbsk = string.IsNullOrWhiteSpace(a.CBSK) ? GeneralUtility.NullConstant : a.CBSK;

                return DbExcutor.ExcuteDataTable(
                     query: $@"FR_009 <# {fromDate} #>, 
                            <# {toDate} #>, 
                            <# {cbsk} #> , 
                            <# {GeneralUtility.NullConstant} #> , 
                            <# {(int)RecordStatus.Post} #>",
                     connection: "FC.Acc",
                     name: "CashbookLogDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CashbookReceiptPaymentReport(Expression<Func<CashbookReceiptPaymentReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(TaxReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);
                    }
                });

                var cbsk = string.IsNullOrWhiteSpace(a.CBSK) ? GeneralUtility.NullConstant : a.CBSK;
                var transType = a.TransType != null ? a.TransType.ToString() : GeneralUtility.NullConstant;

                return DbExcutor.ExcuteDataTable(
                     query: $@"FR_009 <# {fromDate} #>, 
                            <# {toDate} #>, 
                            <# {cbsk} #> , 
                            <# {transType} #> , 
                            <# {(int)RecordStatus.Post} #>",
                     connection: "FC.Acc",
                     name: "CashbookReceiptPaymentDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
