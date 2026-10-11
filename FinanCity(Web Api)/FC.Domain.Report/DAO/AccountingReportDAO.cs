namespace FC.Domain.Report.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Report.Filter;

    public class AccountingReportDAO : DAO
    {
        #region Methods

        public DataTable TaxReport(Expression<Func<TaxReportFilter, bool>> filter)
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

                var taxsk = string.IsNullOrWhiteSpace(a.TaxSK) ? GeneralUtility.NullConstant : a.TaxSK;

                return DbExcutor.ExcuteDataTable(
                    query: $"R0001 <# {fromDate} #>, <# {toDate} #>, <# {taxsk} #>",
                    connection: "FC.Acc",
                    name: "GSTDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable GeneralLedgerSummaryReport(Expression<Func<GeneralLedgerReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem != null && mem == $"{nameof(TaxReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);
                    }
                });

                var accountsk = string.IsNullOrWhiteSpace(a.AccountSK) ? GeneralUtility.NullConstant : a.AccountSK;

                return DbExcutor.ExcuteDataTable(
                    query: $@"FR_006 <# {fromDate} #>, 
                                     <# {toDate} #>, 
                                     <# {accountsk} #>",
                    connection: "FC.Acc",
                    name: "GeneralLedgerSummaryReportDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GeneralLedgerRefDataTable(Expression<Func<GeneralLedgerReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem != null && mem == $"{nameof(TaxReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);
                    }
                });

                var accountsk = string.IsNullOrWhiteSpace(a.AccountSK) ? GeneralUtility.NullConstant : a.AccountSK;
                var glref = GeneralUtility.NullConstant;
                var secondRef = GeneralUtility.NullConstant;

                return DbExcutor.ExcuteDataTable(
                    query: $@"FR_008 <# {fromDate} #>, 
                                     <# {toDate} #>, 
                                     <# {accountsk} #>,
                                     <# {glref} #>,
                                     <# {secondRef} #>",
                    connection: "FC.Acc",
                    name: "GeneralLedgerRefDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GeneralLedgerDetailReport(Expression<Func<GeneralLedgerReportFilter, bool>> filter)
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

                var accountsk = string.IsNullOrWhiteSpace(a.AccountSK)
                    ? GeneralUtility.NullConstant : a.AccountSK;

                return DbExcutor.ExcuteDataTable(
                    query: $@"FR_005 <# {fromDate} #>, 
                                     <# {toDate} #>, 
                                     <# {accountsk} #>",
                                    
                    connection: "FC.Acc",
                    name: "GeneralLedgerDetailReportDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GeneralLedgerAuditReport(Expression<Func<GeneralLedgerAiditReportFilter, bool>> filter)
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

                var accountsk = string.IsNullOrWhiteSpace(a.AccountSK)
                    ? GeneralUtility.NullConstant : a.AccountSK;

                var glref = string.IsNullOrWhiteSpace(a.GLRef)
                    ? GeneralUtility.NullConstant : a.GLRef;

                var secondref = string.IsNullOrWhiteSpace(a.SecondRef)
                 ? GeneralUtility.NullConstant : a.SecondRef;

                return DbExcutor.ExcuteDataTable(
                    query: $@"FR_005 <# {fromDate} #>, 
                                     <# {toDate} #>, 
                                     <# {accountsk} #>,
                                     <# {secondref} #>,
                                     <# {glref} #>",

                    connection: "FC.Acc",
                    name: "GeneralLedgerDetailReportDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<FinantialStatementReportDTO> FindFinantialStatementReportDTO(
            string accountingYear,
            string fromPeriod,
            string toPeriod)
        {
            try
            {
                return DbExcutor<FinantialStatementReportDTO>.ExcuteReader($@"FR_001 <# {accountingYear} #>,
                    <# {fromPeriod} #>, 
                    <# {toPeriod} #>, 
                    <# {GeneralUtility.NullConstant} #>,
                    <# {CommonConstant.AccountType.INCOME_START} #>,
                    <# {CommonConstant.AccountType.OTHER_EXPENSE_END} #>");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
