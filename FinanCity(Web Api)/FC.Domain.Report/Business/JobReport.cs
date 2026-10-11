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

    public class JobReport : BusinessLogic<JobReportDAO>
    {

        #region Fields

        AccountingPeriod _accountingPeriod;

        #endregion

        #region Ctor

        public JobReport(AccountingPeriod accountingPeriod)
        {
            _accountingPeriod = accountingPeriod;
        }

        #endregion

        #region Methods

        public DataTable JobActivityDetailReport(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                return DAO.JobDetailReportDTO(filter: filter)
                    .OrderBy(x => x.AccSK)
                    .ToDataTable("JobActivityDetailDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable JobActivitySummaryReport(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                var data = DAO.JobDetailReportDTO(filter: filter)
                    .OrderBy(x => x.AccSK);

                return data.GroupBy(g => new
                {
                    AccSK = g.AccSK,
                    AccCode = g.AccCode,
                    AccDescription = g.AccDescription,
                    JobSK = g.JobSK,
                    JobNo = g.JobNo,
                    JobName = g.JobName,

                }).Select(x => new JobDetailReportDTO
                {
                    AccSK = x.Key.AccSK,
                    AccCode = x.Key.AccCode,
                    AccDescription = x.Key.AccDescription,
                    JobSK = x.Key.JobSK,
                    JobName = x.Key.JobName,
                    JobNo = x.Key.JobNo,
                    Debit = data.Where(y => y.AccSK == x.Key.AccSK && y.JobSK == x.Key.JobSK).Sum(y => y.Debit.ToRoundUp()),
                    Credit = decimal.Negate(data.Where(y => y.AccSK == x.Key.AccSK && y.JobSK == x.Key.JobSK).Sum(y => y.Credit.ToRoundUp()))

                }).OrderBy(x => x.AccSK).ToDataTable("JobActivitySummaryDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable JobActivityListReport(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                var summary = DAO.JobSummaryReport(filter: filter);

                var result = summary.GroupBy(x => new
                {
                    x.JobSK,
                    x.JobNo,
                    x.JobName,
                })
                 .Select(x => new JobListReportDTO
                 {
                     JobName = x.Key.JobName,
                     JobSK = x.Key.JobSK,
                     JobNo = x.Key.JobNo,

                     Income = summary.Where(y => int.Parse(y.AccSK) >= int.Parse(CommonConstant.AccountType.INCOME_START) && int.Parse(y.AccSK) <= int.Parse(CommonConstant.AccountType.INCOME_END))
                        .Where(a => a.JobSK == x.Key.JobSK).Sum(y => y.TransactionAmount),

                     COS = summary.Where(y => int.Parse(y.AccSK) >= int.Parse(CommonConstant.AccountType.COGS_START) && int.Parse(y.AccSK) <= int.Parse(CommonConstant.AccountType.COGS_END))
                        .Where(a => a.JobSK == x.Key.JobSK).Sum(y => y.TransactionAmount),

                     Expense = summary.Where(y => int.Parse(y.AccSK) >= int.Parse(CommonConstant.AccountType.EX_START) && int.Parse(y.AccSK) <= int.Parse(CommonConstant.AccountType.EX_END))
                        .Where(a => a.JobSK == x.Key.JobSK).Sum(y => y.TransactionAmount),

                     OtherIncome = summary.Where(y => int.Parse(y.AccSK) >= int.Parse(CommonConstant.AccountType.OTHER_INCOME_START) && int.Parse(y.AccSK) <= int.Parse(CommonConstant.AccountType.OTHER_INCOME_END))
                        .Where(a => a.JobSK == x.Key.JobSK).Sum(y => y.TransactionAmount),

                     OtherExpense = summary.Where(y => int.Parse(y.AccSK) >= int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_START) && int.Parse(y.AccSK) <= int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_END))
                        .Where(a => a.JobSK == x.Key.JobSK).Sum(y => y.TransactionAmount)

                 })
                 .Select(x => new JobListReportDTO
                 {
                     JobName = x.JobName,
                     JobSK = x.JobSK,
                     JobNo = x.JobNo,
                     Income = Math.Abs(x.Income + x.OtherIncome),
                     COS = Math.Abs(x.COS),
                     Expense = Math.Abs(x.Expense + x.OtherExpense),
                     OtherIncome = 0,
                     OtherExpense = 0,
                     NetProfit = Math.Abs(x.Income) + Math.Abs(x.OtherIncome) - Math.Abs(x.COS) - Math.Abs(x.Expense) - Math.Abs(x.OtherExpense)
                 });

                return result.ToDataTable("JobListReportDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
      
        #endregion
    }
}
