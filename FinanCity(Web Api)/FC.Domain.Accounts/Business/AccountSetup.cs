namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure;
    using FC.Domain.Accounts.Business.ChartAccounts;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;

    public class AccountSetup : BusinessLogic<AccountSetupEntity, AccountSetupDAO>
    {
        #region Fields

        private AccountingPeriod _accountingPeriod;
        private BTable _bTable;
        private HierarchyChartBuilder _coaBuilder;

        #endregion

        #region Ctor

        public AccountSetup(BTable bTable,
            AccountingPeriod accountingPeriod,
           HierarchyChartBuilder coaBuilder)
        {
            _accountingPeriod = accountingPeriod;
            _bTable = bTable;
            _coaBuilder = coaBuilder;
        }

        #endregion

        #region Methods

        public IEnumerable<ChartOfAccountDTO> FindCOATable(string yc,
            string fromPeriod = "01",
            string toPeriod = "12",
            string accountPrefix = null)
        {
            try
            {
                Guard.IsNotNullOrEmpty(yc, "yc");

                var startDate = _accountingPeriod.Finds(filter: f => f.YC == yc && f.PC == fromPeriod).First().Date1;
                var endDate = _accountingPeriod.Finds(filter: f => f.YC == yc && f.PC == toPeriod).First().Date2;

                return FindCOATable(
                    fromPeriod: startDate,
                    toPeriod: endDate,
                    accountPrefix: accountPrefix);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public IEnumerable<ChartOfAccountDTO> FindCOATable(
            int? fromPeriod,
            int? toPeriod,
            string accountPrefix = null)
        {
            try
            {
                Guard.IsNotNull(fromPeriod, "fromPeriod");
                Guard.IsNotNull(toPeriod, "toPeriod");

                var chartAccounts = DAO.FindCOATable();

                if (!string.IsNullOrWhiteSpace(accountPrefix))
                {
                    chartAccounts = chartAccounts
                        .Where(x => x.AccountSK.StartsWith(accountPrefix));
                }

                Func<string, IEnumerable<BTableEntity>> findGL = (module) =>
                {
                    if (!string.IsNullOrWhiteSpace(accountPrefix))
                    {
                        var result = _bTable.Finds(filter: f => f.AccSK.StartsWith(accountPrefix)
                          && f.ModuleCode == module
                          && f.PostDate >= fromPeriod
                          && f.PostDate <= toPeriod
                          && f.RecordStatus == (int)RecordStatus.Post);

                        return result;
                    }
                    else
                    {
                        var result = _bTable.Finds(filter: f => f.ModuleCode == module
                        && f.PostDate >= fromPeriod
                        && f.PostDate <= toPeriod
                        && f.RecordStatus == (int)RecordStatus.Post);

                        return result;
                    }
                };

                var cbp = findGL(CommonConstant.Module.MODULE_CASHBOOK_PAYMENT);
                var cbr = findGL(CommonConstant.Module.MODULE_CASHBOOK_RECEIPT);
                var glc = findGL(CommonConstant.Module.MODULE_GENERAL_LEDGER);
                var ap = findGL(CommonConstant.Module.MODULE_PURCHASE);
                var ar = findGL(CommonConstant.Module.MODULE_SALE);
                var stk = findGL(CommonConstant.Module.MODULE_STOCK);
                var ext_1 = findGL(CommonConstant.Module.MODULE_EXTRA_ONE);
                var ext_2 = findGL(CommonConstant.Module.MODULE_EXTRA_TWO);

                var _bTableUnion = (cbp.Union(cbr)
                   .Union(glc)
                   .Union(ap)
                   .Union(ar)
                   .Union(stk)
                   .Union(ext_1)
                   .Union(ext_2))
                   .GroupBy(x => x.AccSK)
                   .Select(x => new BTableExtension
                   {
                       AccSK = x.Key,
                       Debit = x.Sum(a => a.TransactionAmount > 0 ? a.TransactionAmount.ToRoundUp() : 0),
                       Credit = x.Sum(a => a.TransactionAmount < 0 ? Math.Abs(a.TransactionAmount.ToRoundUp()) : 0)
                   });

                _coaBuilder.Build(chartAccounts, _bTableUnion);
                return chartAccounts;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
