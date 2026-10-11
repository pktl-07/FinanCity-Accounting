namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;
    using FC.Core.Infrastructure.Implements.RecordIndexer;

    public class YearClosing : BusinessLogic<YearClosingDAO>
    {
        #region Fields

        private RecordIndexerFactory _recordIndexer;
        private AccountSetup _accountSetup;
        private AccountingPeriod _accountingPeriod;
        private BTable _bTable;
        private BatchControl _batchControl;
    
        #endregion

        #region Ctor

        public YearClosing(AccountingPeriod accountingPeriod,
            RecordIndexerFactory recordIndexer,
            BTable bTable,
            BatchControl batchControl,
            AccountSetup accountSetup)
        {
            _recordIndexer = recordIndexer;
            _batchControl = batchControl;
            _bTable = bTable;
            _accountingPeriod = accountingPeriod;
            _accountSetup = accountSetup;
        }

        #endregion

        #region Methods

        public void CloseCurrentAccountingYear()
        {
            try
            {
                var closingAccount = this.ClosingAccount;
                var currentYearCode = _accountingPeriod.CurrentAccountingYearCode();
                var currentYear = _accountingPeriod.CurrentAccountingYear();
                var newYearCode = (int.Parse(currentYearCode) + 1).ToString("00");
                var app = DI.Current.Get<IEssAppRepository>();

                if (!_accountingPeriod.HasNextAccountingYear())
                    _accountingPeriod.Add12AccountingPeriod();

                var batch = _batchControl.Finds(filter: f =>
                    f.ModuleCode == CommonConstant.Module.MODULE_GENERAL_LEDGER
                    && f.YC == newYearCode).Single();

                var setting = _recordIndexer.Generate(TransType.Journal, closingAccount[0]);
               
                //especially time stamp should all be the same.
                var cretedTime = GeneralUtility.ServerTime;
                var createdDate = GeneralUtility.ServerDate;

                foreach (var info in closingAccount)
                {
                    info.SetTableName(batch.BatchTableName);
                    info.R1 = setting.R1;
                    info.VoucherId = setting.VoucherId;
                    info.Prefix = setting.Prefix;
                    info.Postfix = setting.Postfix;
                    info.TransType = (int)TransType.Journal;
                    info.PostDate = int.Parse($"{currentYear + 1}0101");
                    info.EntryDescription = "OPENING ENTRY";
                    info.CURRSK = app.CURRSK;
                    info.CURRATE = app.CURRATE;
                    info.CURROP = app.CURROP;
                    info.YCPC = $"{newYearCode}00";
                    info.BatchNo = batch.SysKey;
                    info.CreatedTime = cretedTime;
                    info.CreatedDate = createdDate;
                }

                _bTable.BulkInsert(closingAccount);

                _accountingPeriod.CloseCurrentAccountingYear();
                _accountingPeriod.SetDefaultAccountingYear(newYearCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private List<BTableEntity> ClosingAccount
        {
            get
            {
                try
                {
                    var app = DI.Current.Get<IEssAppRepository>();
                    var bTableList = new List<BTableEntity>();
                    var isAlreadyAddRetainedEarning = false;
                    var parentSyskey = GeneralUtility.Key;

                    var yc = _accountingPeriod.CurrentAccountingYearCode();
                    var data = _accountSetup.FindCOATable(yc: _accountingPeriod.CurrentAccountingYearCode());

                    var sales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                    var costOfSales = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.COGS_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                    var otherIncome = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_INCOME_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                    var expense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.EX_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                    var otherExpense = data.Where(x => x.AccountType == (int)AccountType.Posting && x.GroupHeaderType == int.Parse(CommonConstant.AccountType.OTHER_EXPENSE_START)).Sum(x => x.TransactionAmount.ToRoundUp());
                    var beforeTaxation = decimal.Negate(sales - costOfSales + otherIncome - expense - otherExpense);

                    foreach (var entity in data.Where(x => x.AccountType == (int)AccountType.Posting
                        && x.GroupHeaderType >= int.Parse(CommonConstant.AccountType.BS_START)
                        && x.GroupHeaderType <= int.Parse(CommonConstant.AccountType.BS_END)
                        && x.TransactionAmount.ToRoundUp() != 0))
                    {
                        var bTableEntity = new BTableEntity();

                        bTableEntity.ParentId = parentSyskey;
                        bTableEntity.ModuleCode = CommonConstant.Module.MODULE_GENERAL_LEDGER;
                        bTableEntity.RecordStatus = (int)RecordStatus.Post;
                        bTableEntity.AccSK = entity.AccountSK;
                        bTableEntity.TransactionAmount = entity.Debit - entity.Credit;
                        bTableEntity.LineDescription = "Opening entry";
                        bTableEntity.EntryDescription = "Opening entry";
                        bTableEntity.SourceHeaderTransSK = bTableEntity.SourceHeaderTransSK;
                        bTableEntity.SecondRef = bTableEntity.SecondRef;
                        bTableEntity.HasTaxLine = false;
                        bTableEntity.CURRSK = app.CURRSK;
                        bTableEntity.CURROP = app.CURROP;
                        bTableEntity.CURRATE = app.CURRATE;

                        //Important!! this post into the retain earning
                        if (bTableEntity.AccSK == app.DefaultAccountingCode.RetaninEarning.Key)
                        {
                            isAlreadyAddRetainedEarning = true;
                            bTableEntity.TransactionAmount = bTableEntity.TransactionAmount.ToRoundUp() + beforeTaxation;
                        }

                        bTableEntity.IsAutoOpening = true;
                        bTableList.Add(bTableEntity);
                    }

                    if (!isAlreadyAddRetainedEarning)
                    {
                        var bTableEntity = new BTableEntity();

                        bTableEntity.ParentId = parentSyskey;
                        bTableEntity.ModuleCode = CommonConstant.Module.MODULE_GENERAL_LEDGER;
                        bTableEntity.RecordStatus = (int)RecordStatus.Post;
                        bTableEntity.AccSK = app.DefaultAccountingCode.RetaninEarning.Key;
                        bTableEntity.TransactionAmount = beforeTaxation;
                        bTableEntity.LineDescription = app.DefaultAccountingCode.RetaninEarning.Value;
                        bTableEntity.IsAutoOpening = true;
                        bTableEntity.HasTaxLine = false;
                        bTableEntity.CURRSK = app.CURRSK;
                        bTableEntity.CURROP = app.CURROP;
                        bTableEntity.CURRATE = app.CURRATE;

                        bTableList.Add(bTableEntity);
                    }

                    return bTableList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion
    }
}
