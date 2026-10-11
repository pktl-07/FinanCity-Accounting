namespace FC.Domain.Report.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Report.Filter;

    public class JobReportDAO : DAO
    {
        #region Methods

        public IEnumerable<JobSummaryReportDTO> JobSummaryReport(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;
                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(JobActivityDatailReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                var jobsk = string.IsNullOrWhiteSpace(a.JobSK) ? GeneralUtility.NullConstant :
                    a.JobSK;

                var query = $@"FR_002 
                            <# {fromDate} #>,
                            <# {toDate} #>,
                            <# {GeneralUtility.NullConstant} #>,
                            <# {CommonConstant.AccountType.BS_START} #>,
                            <# {CommonConstant.AccountType.OTHER_EXPENSE_END} #>,
                            <# {jobsk} #>";

                return DbExcutor<JobSummaryReportDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<JobDetailReportDTO> JobDetailReportDTO(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;
                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(JobActivityDatailReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                var jobsk = string.IsNullOrWhiteSpace(a.JobSK) ? GeneralUtility.NullConstant :
                    a.JobSK;

                var query = $@"FR_003 
                            <# {fromDate} #>,
                            <# {toDate} #>,
                            <# {GeneralUtility.NullConstant} #>,
                            <# {CommonConstant.AccountType.BS_START} #>,
                            <# {CommonConstant.AccountType.OTHER_EXPENSE_END} #>,
                            <# {jobsk} #>";

                return DbExcutor<JobDetailReportDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
                var transType = a.TransType != null ? GeneralUtility.NullConstant : a.TransType.ToString();

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
