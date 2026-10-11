namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
   
    public class AccountingPeriod : BusinessLogic<AccountingPeriodEntity, AccountingPeriodDAO>
    {
        #region Fields

        private BatchModule _batchModule;
        private BatchControl _batchControl;
      
        #endregion

        #region Ctor

        public AccountingPeriod(BatchModule batchModule,
            BatchControl batchControl)
        {
            _batchModule = batchModule;
            _batchControl = batchControl;
        }

        #endregion

        #region Methods

        public void Add12AccountingPeriod()
        {
            try
            {
                var lastYear = LastAccountingYear() == null ? GeneralUtility.ServerYear - 1 :
                    LastAccountingYear().Value;

                var period = new AccountingPeriodEntity
                {
                    YC = LastAccountingYearCode(),
                    Date1 = int.Parse($"{lastYear}0000"),
                    Date2 = int.Parse($"{lastYear}1231"),
                };

                var periods = Get12AccountingPeriod(period);
                var batches = GetBatchModuleList(periods);
                var controls = GetBatchControlList(periods);

                BulkInsert(periods);

                _batchModule.BulkInsert(batches);
                _batchControl.BulkInsert(controls);

                foreach (var c in controls)//To create BTable
                {
                    _batchControl.CreateBTable(c.BatchTableName);
                }

                if (Finds(filter: f => f.IsDefaultYear == true).Count() == 0)
                {
                    SetDefaultAccountingYear("01");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<AccountingPeriodDTO> GetAccountingPeriodDTO(string YC)
        {
            try
            {
                return DAO.GetAccountingPeriodDTO(YC);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string LastAccountingYearCode()
        {
            try
            {
                return DAO.LastAccountingYearCode();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int? LastAccountingYear()
        {
            try
            {
                return DAO.LastAccountingYear();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string CurrentAccountingYearCode()
        {
            try
            {
                return DAO.CurrentAccountingYearCode();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool HasNextAccountingYear()
        {
            try
            {
                var currentYear = CurrentAccountingYearCode();
                var nextYear = (int.Parse(currentYear) + 1).ToString("00");

                return Finds(filter: f => f.YC == nextYear).Count() > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int? CurrentAccountingYear()
        {
            try
            {
                return DAO.CurrentAccountingYear();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CloseCurrentAccountingYear()
        {
            try
            {
                var periods = Finds(filter: f => f.IsDefaultYear == true);
                var currentYearCode = periods.First().YC;

                periods.Each(period =>
                {
                    period.BatchEntryStatus = (int)EntryStatus.UnChangeClose;
                    period.IsDefaultYear = false;
                    Update(entity: period, filter: f => f.SysKey == period.SysKey);
                });

                _batchModule.Finds(filter: f => f.YC == currentYearCode).Each(module =>
                {
                    module.EntryStatus = (int)EntryStatus.UnChangeClose;
                    _batchModule.Update(entity: module, filter: f => f.SysKey == module.SysKey);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SetDefaultAccountingYear(string yearCode)
        {
            try
            {
                Finds(filter: f => f.YC == yearCode).Each(period =>
                {
                    period.BatchEntryStatus = (int)EntryStatus.Open;
                    period.IsDefaultYear = true;

                    Update(entity: period, filter: f => f.SysKey == period.SysKey);
                });

                _batchModule.Finds(filter: f => f.YC == yearCode)
                    .Each(module =>
                {
                    module.EntryStatus = (int)EntryStatus.Open;
                    _batchModule.Update(entity: module, filter: f => f.SysKey == module.SysKey);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private IEnumerable<AccountingPeriodEntity> Get12AccountingPeriod(AccountingPeriodEntity accountingPeriodEntity)
        {
            var result = new List<AccountingPeriodEntity>();
            var accountingDate = ((DateTime)GeneralUtility.ConvertDateFormat(accountingPeriodEntity.Date2.ToString())).AddDays(1);
            var yearCode = Convert.ToInt16(accountingPeriodEntity.YC) + 1;

            for (int _period = 0; _period <= 12; _period++)
            {
                var accountingPeriod = new AccountingPeriodEntity();
                accountingPeriod.YC = yearCode.ToString("00");
                accountingPeriod.PC = _period.ToString("00");

                if (_period == 0)
                {
                    accountingPeriod.Date1 = int.Parse(accountingDate.Year.ToString() + "0000");
                    accountingPeriod.Date2 = int.Parse(accountingDate.Year.ToString() + "0000");
                }
                else
                {
                    accountingPeriod.Date1 = int.Parse(GeneralUtility.ConvertSystemDateStringFormat(accountingDate));
                    accountingPeriod.Date2 = int.Parse(GeneralUtility.ConvertSystemDateStringFormat(accountingDate.AddMonths(1).AddDays(-1)));
                }

                result.Add(accountingPeriod);

                if (_period != 0)
                {
                    accountingDate = accountingDate.AddMonths(1);
                }
            }
            return result;
        }

        private List<BatchModuleEntity> GetBatchModuleList(IEnumerable<AccountingPeriodEntity> accoutingPeriodList)
        {
            var result = new List<BatchModuleEntity>();

            foreach (var v in accoutingPeriodList)
            {
                for (int i = 0; i < 8; i++)
                {
                    var batchModuleEntity = new BatchModuleEntity();

                    batchModuleEntity.YC = v.YC;
                    batchModuleEntity.PC = v.PC;
                    batchModuleEntity.EntryStatus = (int)EntryStatus.Open;

                    switch (i)
                    {
                        case 0:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_GENERAL_LEDGER;
                            break;
                        case 1:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT;
                            break;
                        case 2:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT;
                            break;
                        case 3:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_SALE;
                            break;
                        case 4:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_PURCHASE;
                            break;
                        case 5:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_STOCK;
                            break;
                        case 6:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_EXTRA_ONE;
                            break;
                        case 7:
                            batchModuleEntity.ModuleCode = CommonConstant.Module.MODULE_EXTRA_TWO;
                            break;
                    }
                    result.Add(batchModuleEntity);
                }
            }
            return result;
        }

        private List<BatchControlEntity> GetBatchControlList(IEnumerable<AccountingPeriodEntity> accoutingPeriodList)
        {
            var batchControlList = new List<BatchControlEntity>();
            var maximumBatchTable = _batchControl.MaxBatchTable();

            if (maximumBatchTable == null)
            {
                maximumBatchTable = new BatchControlEntity();
                maximumBatchTable.BatchTableName = "B" + "0000000";
            }

            var yearCode = accoutingPeriodList.Select(x => new { x.YC }).GroupBy(x => x);
            var maxBtable = Convert.ToInt64(maximumBatchTable.BatchTableName.ToString().Substring(1));

            foreach (var v in yearCode)
            {
                for (int i = 0; i < 8; i++)
                {
                    var batchEntity = new BatchControlEntity();

                    maxBtable += 1;

                    batchEntity.BatchTableName = "B" + maxBtable.ToString("0000000");
                    batchEntity.YC = v.Key.YC.ToString();

                    switch (i)
                    {
                        case 0:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_GENERAL_LEDGER;
                            batchEntity.Description = "General Ledger";
                            break;
                        case 1:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT;
                            batchEntity.Description = "Payment";
                            break;
                        case 2:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT;
                            batchEntity.Description = "Receipt";
                            break;
                        case 3:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_SALE;
                            batchEntity.Description = "Sales A/R";
                            break;
                        case 4:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_PURCHASE;
                            batchEntity.Description = "Purchase A/P";
                            break;
                        case 5:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_STOCK;
                            batchEntity.Description = "Stock";
                            break;
                        case 6:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_EXTRA_ONE;
                            batchEntity.Description = "Extra One";
                            break;
                        case 7:
                            batchEntity.ModuleCode = CommonConstant.Module.MODULE_EXTRA_TWO;
                            batchEntity.Description = "Extra Two";
                            break;
                    }

                    batchControlList.Add(batchEntity);
                }
            }
            return batchControlList;
        }

        #endregion
    }
}
