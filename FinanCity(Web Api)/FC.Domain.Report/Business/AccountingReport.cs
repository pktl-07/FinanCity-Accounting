namespace FC.Domain.Report.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Report.DAO;
    using FC.Domain.Report.Filter;

    public class AccountingReport : BusinessLogic<AccountingReportDAO>
    {
        #region Fields

        private AccountingPeriod _accountingPeriod;
        private AccountSetup _accountSetup;
        private BTable _journalLedger;

        #endregion

        #region Ctor
        public AccountingReport(AccountingPeriod accountingPeriod,
          AccountSetup accountSetup,
          BTable journalLedger)
        {
            _journalLedger = journalLedger;
            _accountSetup = accountSetup;
            _accountingPeriod = accountingPeriod;
        }

        #endregion

        #region Methods

        public DataSet BalanceSheetReport(Expression<Func<AccountingReportFilter, bool>> filter,
          out string beforeTax,
          bool includeAccountHeader = true)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(AccountingReportFilter.AccountingDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                var data = _accountSetup.FindCOATable(
                    fromPeriod: fromDate,
                    toPeriod: toDate);

                if (!includeAccountHeader)
                    data = data.Where(x => x.AccountType == 1);

                var sales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var costOfSales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.COGS_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherIncome = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var expense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.EX_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherExpense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var beforeTaxation = sales - costOfSales + otherIncome - expense - otherExpense;

                if (beforeTaxation < 0)
                    beforeTax = string.Format("({0:N2})", decimal.Negate(beforeTaxation));
                else
                    beforeTax = beforeTaxation.ToString("N2");

                var dataSet = new DataSet();
                dataSet.Tables.Add(data.ToDataTable("COADataTable"));

                return dataSet;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet TrialBalanceReport(Expression<Func<AccountingReportFilter, bool>> filter)
        {
            try
            {
                IEnumerable<AccountingPeriodEntity> period;

                var data = this.FindCAOTable(filter: filter,
                    period: out period)
                    .Where(x => x.AccountType == (int)AccountType.Posting);
             
                var yc = period.First().YC;
                var startPeriod = period.First().PC;
                var endPeriod = period.Last().PC;
                var full = period.First().Date1.ToString().Substring(0, 4);

                var sales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var costOfSales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.COGS_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherIncome = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var expense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.EX_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherExpense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var beforeTaxation = sales - costOfSales + otherIncome - expense - otherExpense;

                var dataSet = new DataSet();
                dataSet.Tables.Add(data.ToDataTable("COADataTable"));

                return dataSet;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet IncomeStatementReport(Expression<Func<AccountingReportFilter, bool>> filter, out string beforeTax)
        {
            try
            {
                IEnumerable<AccountingPeriodEntity> period;

                var data = this.FindCAOTable(filter: filter,
                    period: out period);

                var sales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var costOfSales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.COGS_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherIncome = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var expense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.EX_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var otherExpense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                var beforeTaxation = sales - costOfSales + otherIncome - expense - otherExpense;

                if (beforeTaxation < 0)
                    beforeTax = string.Format("({0:N2})", decimal.Negate(beforeTaxation));
                else
                    beforeTax = beforeTaxation.ToString("N2");

                var dataSet = new DataSet();
                dataSet.Tables.Add(data.ToDataTable("COADataTable"));

                return dataSet;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable TaxReport(Expression<Func<TaxReportFilter, bool>> filter)
        {
            try
            {
                return DAO.TaxReport(filter: filter);
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
                var dataTabe = DAO.GeneralLedgerSummaryReport(filter: filter);

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

                for (int i = 0; i < dataTabe.Rows.Count; i++)
                {
                    dataTabe.Rows[i]["OpeningBalance"] = _journalLedger.GetOpeningBalance(date: fromDate.Value, dataTabe.Rows[i]["AccSK"].ToString());
                    dataTabe.Rows[i]["Credit"] = decimal.Negate((decimal)dataTabe.Rows[i]["Credit"]);
                    dataTabe.Rows[i]["ClosingBalance"] =
                        (decimal)dataTabe.Rows[i]["OpeningBalance"] +
                        (decimal)dataTabe.Rows[i]["Debit"] -
                        (decimal)dataTabe.Rows[i]["Credit"];
                }
                return dataTabe;
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
                var dataTabe = DAO.GeneralLedgerRefDataTable(filter: filter);

                for (int i = 0; i < dataTabe.Rows.Count; i++)
                    dataTabe.Rows[i]["Credit"] = decimal.Negate((decimal)dataTabe.Rows[i]["Credit"]);
               
                return dataTabe;
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
                var dt = DAO.GeneralLedgerDetailReport(filter: filter);
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

                _accountSetup.Finds(x => x.AccountType == (int)AccountType.Posting)
                    .Each(acc =>
                    {
                        var op = _journalLedger.GetOpeningBalance(date: fromDate.Value, acc.SysKey);

                        dt.AsEnumerable()
                        .Where(x => x.Field<string>("AccSK") == acc.SysKey)
                        .Each(r =>
                        {
                            var credit = decimal.Negate(r.Field<decimal>("Credit")).ToRoundUp();
                            var debit = r.Field<decimal>("Debit").ToRoundUp();
                            var cs = op + (debit - credit);

                            r.SetField("OpeningBalance", op);
                            r.SetField("Debit", debit);
                            r.SetField("Credit", credit);
                            r.SetField("ClosingBalance", cs);

                            op = cs;
                        });
                    });

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable IncomeStatementYearlyReport(
          Expression<Func<CompareYearlyIncomeStatementFilter, bool>> filter)
        {
            try
            {
                var a = filter.CreateObject();

                var selectedYearResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}00",
                    toPeriod: $"{a.YCStart}12");

                var comparedYearResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}00",
                    toPeriod: $"{a.YCStart}12");

                #region Union Group By

                var union = selectedYearResult
                    .Union(comparedYearResult)
                    .GroupBy(x => new
                    {
                        x.AccSK,
                        x.CatDescription,
                        x.AccDescription,
                        x.AccCode,
                        x.CatSK,
                    })
                    .Select(x => new FinantialStatementReportDTO
                    {
                        AccSK = x.Key.AccSK,
                        CatDescription = x.Key.CatDescription,
                        AccDescription = x.Key.AccDescription,
                        AccCode = x.Key.AccCode,
                        CatSK = x.Key.CatSK,
                    });

                #endregion

                #region Projection

                return union.Select(x => new
                {
                    AccSK = x.AccSK,
                    CatDescription = x.CatDescription,
                    AccDescription = x.AccDescription,
                    AccCode = x.AccCode,
                    CatSK = x.CatSK,
                    YC = x.YC,
                    PC = x.PC,
                    SelectedYearAmount = GetTransactionAmountByPeriod(x.AccSK, selectedYearResult),
                    CompareYearAmount = GetTransactionAmountByPeriod(x.AccSK, comparedYearResult),
                }).ToList().ToDataTable("IncomeStatementCompareDataTable");

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable IncomeStatementQuarterlyReport(
          Expression<Func<CompareQuarterlyIncomeStatementReportFilter, bool>> filter)
        {
            try
            {
                var a = filter.CreateObject();

                #region Selected And Compared Year Data

                var firstPeriodResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}00",
                    toPeriod: $"{a.YCStart}03");

                var secondPeriodResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}04",
                    toPeriod: $"{a.YCStart}06");

                var thirdPeriodResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}07",
                    toPeriod: $"{a.YCStart}19");

                var fourthPeriodResult = DAO.FindFinantialStatementReportDTO(
                    accountingYear: a.YCStart,
                    fromPeriod: $"{a.YCStart}09",
                    toPeriod: $"{a.YCStart}12");

                #endregion

                #region Union Group By

                var union = firstPeriodResult
                    .Union(secondPeriodResult)
                    .Union(thirdPeriodResult)
                    .Union(fourthPeriodResult)
                    .GroupBy(x => new
                    {
                        x.AccSK,
                        x.CatDescription,
                        x.AccDescription,
                        x.AccCode,
                        x.CatSK,
                    })
                    .Select(x => new FinantialStatementReportDTO
                    {
                        AccSK = x.Key.AccSK,
                        CatDescription = x.Key.CatDescription,
                        AccDescription = x.Key.AccDescription,
                        AccCode = x.Key.AccCode,
                        CatSK = x.Key.CatSK,
                    });

                #endregion

                #region Projection

                return union.Select(x => new
                {
                    AccSK = x.AccSK,
                    CatDescription = x.CatDescription,
                    AccDescription = x.AccDescription,
                    AccCode = x.AccCode,
                    CatSK = x.CatSK,
                    YC = x.YC,
                    PC = x.PC,
                    FirstPeriod = GetTransactionAmountByPeriod(x.AccSK, firstPeriodResult),
                    SecondPeriod = GetTransactionAmountByPeriod(x.AccSK, secondPeriodResult),
                    ThirdPeriod = GetTransactionAmountByPeriod(x.AccSK, thirdPeriodResult),
                    FourthPeriod = GetTransactionAmountByPeriod(x.AccSK, fourthPeriodResult),
                }).ToList().ToDataTable("QuarterlyIncomeStatementCompareDataTable1");

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable IncomeStatementMonthlyReport(
         Expression<Func<CompareMonthlyIncomeStatementFilter, bool>> filter)
        {
            try
            {
                #region Selected And Compared Monthly Data
                var a = filter.CreateObject();
                var result = new List<FinantialStatementReportDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    result.AddRange(DAO.FindFinantialStatementReportDTO(
                        accountingYear: a.YCStart,
                        fromPeriod: $"{a.YCStart}{i.ToString("00")}",
                        toPeriod: $"{a.YCStart}{i.ToString("00")}"));
                }

                #endregion

                #region Union Group By

                var union = result
                    .GroupBy(x => new
                    {
                        x.AccSK,
                        x.CatDescription,
                        x.AccDescription,
                        x.AccCode,
                        x.CatSK,
                    })
                    .Select(x => new FinantialStatementReportDTO
                    {
                        AccSK = x.Key.AccSK,
                        CatDescription = x.Key.CatDescription,
                        AccDescription = x.Key.AccDescription,
                        AccCode = x.Key.AccCode,
                        CatSK = x.Key.CatSK,
                    });

                #endregion

                #region Projection

                return union.Select(x => new
                {
                    AccSK = x.AccSK,
                    CatDescription = x.CatDescription,
                    AccDescription = x.AccDescription,
                    AccCode = x.AccCode,
                    CatSK = x.CatSK,
                    YC = x.YC,
                    PC = x.PC,
                    P1 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "01")),
                    P2 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "02")),
                    P3 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "03")),
                    P4 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "04")),
                    P5 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "05")),
                    P6 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "06")),
                    P7 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "07")),
                    P8 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "08")),
                    P9 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "09")),
                    P10 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "10")),
                    P11 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "11")),
                    P12 = GetTransactionAmountByPeriod(x.AccSK, result.Where(p => p.PC == "12")),

                }).ToList().ToDataTable("MonthlyIncomeStatementReportDataTable");

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private IEnumerable<ChartOfAccountDTO> FindCAOTable(
           Expression<Func<AccountingReportFilter, bool>> filter,
           out IEnumerable<AccountingPeriodEntity> period)
        {
            try
            {
                var firstDate = 0;
                var secondDate = 0;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == "AccountingDate" && firstDate == 0)
                        firstDate = int.Parse(value);
                    else if (mem == "AccountingDate" && secondDate == 0)
                        secondDate = int.Parse(value);
                    //var k = typeof(IAccountingDate).IsAssignableFrom(typeof(AccountingReportFilter));
                });

                period = _accountingPeriod.Finds(filter: f => f.Date1 >= firstDate && f.Date2 <= secondDate).ToList();

                return _accountSetup.FindCOATable(
                     yc: period.First().YC,
                     fromPeriod: period.First().PC,
                     toPeriod: period.Last().PC);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Func<string, IEnumerable<FinantialStatementReportDTO>, decimal> GetTransactionAmountByPeriod = (accsk, periodResult) => // filter sum for selected year 
        {
            var result = periodResult.Where(x => x.AccSK == accsk);
            if (result.Count() > 0)
            {
                var a = int.Parse(accsk);
                var incomeStart = int.Parse(CommonConstant.AccountType.INCOME_START);
                var incomeEnd = int.Parse(CommonConstant.AccountType.INCOME_END);
                var otherIncomeStart = int.Parse(CommonConstant.AccountType.OTHER_INCOME_START);
                var otherIncomeEnd = int.Parse(CommonConstant.AccountType.OTHER_INCOME_END);

                if ((a >= incomeStart && a < incomeEnd) || (a >= otherIncomeStart && a < otherIncomeEnd))
                    return decimal.Negate(result.Sum(x => x.TransactionAmount.ToRoundUp()));

                return result.Sum(x => x.TransactionAmount.ToRoundUp());
            }
            return 0;
        };

        #endregion
    }
}
