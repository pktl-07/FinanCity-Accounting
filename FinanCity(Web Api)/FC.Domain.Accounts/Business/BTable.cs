namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
     
    public class BTable : BusinessLogic<BTableEntity, BTableDAO>
    {
        #region Fields

        private RecordIndexerFactory _recordIndexer;
        private AccountingPeriod _accountingPeriod;
        private BatchControl _batchControl;
        private BatchModule _batchModule;
        private IMultiCurrencyConverter _currencyConverter;

        #endregion

        #region Ctor

        public BTable(AccountingPeriod accountingPeriod,
            IMultiCurrencyConverter currencyConverter,
            RecordIndexerFactory recordIndexer,
            BatchModule batchModule,
            BatchControl batchControl)
        {
            _currencyConverter = currencyConverter;
            _batchModule = batchModule;
            _recordIndexer = recordIndexer;
            _accountingPeriod = accountingPeriod;
            _batchControl = batchControl;
        }

        #endregion

        #region Methods

        public string InsertAsTransaction(IEnumerable<BTableEntity> data,
            string moduleCode,
            int? postDate,
            TransType transType,
            string createTime = null,
            int recordStatus = (int)RecordStatus.Save)
        {
            try
            {
                Guard.IsNotNullOrEmpty(moduleCode, "moduleCode");
           
                Validate(data);

                if (createTime == null)
                    createTime = GeneralUtility.ServerTime;

                BindData(data: ref data,
                    moduleCode: moduleCode,
                    postDate: postDate.Value,
                    transType: transType,
                    requireToGenerateId: true,
                    recordStatus : recordStatus);

                data.Each(d =>
                {
                    Insert(d);
                });

                return data.First().ParentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAsTransaction(IEnumerable<BTableEntity> data,
            string moduleCode,
            string parentId,
            int postDate,
            TransType transType,
            int recordStatus = (int)RecordStatus.Update)
        {
            try
            {
                this.UpdateTransaction(data: data,
                    moduleCode: moduleCode, parentId: parentId,
                    postDate: postDate,
                    transType: transType,
                    status: (RecordStatus)recordStatus);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(IEnumerable<BTableEntity> data,
            string moduleCode,
            string parentId,
            int postDate,
            TransType transType)
        {
            try
            {
                var period = _accountingPeriod.Finds(x => x.Date1 <= postDate && x.Date2 >= postDate).Single();
                var batch = _batchControl.Finds(x => x.YC == period.YC && x.ModuleCode == moduleCode).Single();
                
                this.UpdateTransaction(data: data,
                   moduleCode: moduleCode, parentId: parentId,
                   postDate: postDate,
                   transType: transType,
                   status: RecordStatus.Post);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void VoidAsTransaction(
            string moduleCode,
            string parentId,
            int postDate)
        {
            try
            {
                var data = Finds(filter: f =>
                 f.PostDate == postDate &&
                 f.ModuleCode == moduleCode &&
                 f.ParentId == parentId);

                var period = _accountingPeriod.Finds(x => x.Date1 <= postDate && x.Date2 >= postDate).Single();
                var batch = _batchControl.Finds(x => x.YC == period.YC && x.ModuleCode == moduleCode).Single();
                var clone = _currencyConverter.ToMultiCurrency(data).CloneJson();
                var createdDate = GeneralUtility.ServerDate;
                var createdTime = GeneralUtility.ServerTime;

                clone.Each(d =>
                {
                    d.CreatedDate = createdDate;
                    d.CreatedTime = createdTime;
                    d.RecordStatus = (int)RecordStatus.Void;
                    d.SetTableName(batch.BatchTableName);

                    Update(d, filter: f => f.SysKey == d.SysKey);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReverseAsTransaction(
                string moduleCode,
                string parentId,
                int postDate ,
                TransType transType)
        {
            try
            {
                var data = Finds(filter: f =>
                f.PostDate == postDate &&
                f.ModuleCode == moduleCode &&
                f.ParentId == parentId);

                BindData(data: ref data,
                    moduleCode: moduleCode,
                    postDate: postDate,
                    transType: transType,
                    requireToGenerateId: false);

                var clone = _currencyConverter
                    .ToMultiCurrency(data)
                    .CloneJson();

                var period = _accountingPeriod.Finds(filter: f => f.Date1 <= postDate && f.Date2 >= postDate).Single();
                var batch = _batchControl.Finds(x => x.YC == period.YC && x.ModuleCode == moduleCode).Single();
                var reversalParentId = GeneralUtility.Key;
                var reversedParentId = GeneralUtility.Key;
              
                if (moduleCode != CommonConstant.Module.MODULE_GENERAL_LEDGER)
                {
                   
                    UpdateTransaction(data: clone,
                       moduleCode: moduleCode,
                       parentId: parentId,
                       postDate: postDate,
                       transType: transType,
                       status: RecordStatus.Reversal);

                    clone.Each(d =>
                    {
                        d.SysKey = GeneralUtility.Key;
                        d.TransactionAmount = decimal.Negate(d.TransactionAmount);
                        d.ParentId = reversedParentId;
                        d.CreatedDate = GeneralUtility.ServerDate;
                        d.CreatedTime = GeneralUtility.ServerTime;
                        d.RecordStatus = (int)RecordStatus.Reversed;
                        d.SetTableName(batch.BatchTableName);
                        Insert(d);
                    });
                }
                else if (moduleCode == CommonConstant.Module.MODULE_GENERAL_LEDGER)
                {
                    //update data
                    this.UpdateTransaction(data: clone,
                        moduleCode: moduleCode,
                        parentId: parentId,
                        postDate: postDate,
                        transType: transType,
                        status: RecordStatus.Update);

                    clone.Each(d =>
                    {
                        d.SysKey = GeneralUtility.Key;
                        d.TransactionAmount = d.TransactionAmount;
                        d.ParentId = reversedParentId;
                        d.RecordStatus = (int)RecordStatus.Reversal;
                        d.SetTableName(batch.BatchTableName);

                        Insert(d);
                    });

                    clone.Each(d =>
                    {
                        d.SysKey = GeneralUtility.Key;
                        d.TransactionAmount = decimal.Negate(d.TransactionAmount);
                        d.ParentId = reversedParentId;
                        d.CreatedDate = GeneralUtility.ServerDate;
                        d.CreatedTime = GeneralUtility.ServerTime;
                        d.RecordStatus = (int)RecordStatus.Reversed;
                        d.SetTableName(batch.BatchTableName);

                        Insert(d);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IDataPagerResult<JournalSummaryDTO> GetPager(int currentPage, 
            int pageSize, Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetDataPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<JournalSummaryDTO> GetAuditDataPager(int currentPage,
            int pageSize, Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetAuditDataPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override IEnumerable<BTableEntity> Finds(Expression<Func<BTableEntity, dynamic>> selector = null,
          Expression<Func<BTableEntity, bool>> filter = null)
        {
            try
            {
                var query = "";
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((member, value) =>
                {
                    if (member == $"{nameof(BTableEntity.PostDate)}")
                    {
                        if (fromDate == null) fromDate = int.Parse(value);
                        else if (toDate == null) toDate = int.Parse(value);
                    }
                });

                Guard.IsNotNullOrEmpty(a.ModuleCode, "ModuleCode");
                Guard.IsNotNull(fromDate, "fromDate");

                var batchTable = _batchControl.GetBTableByDateRange(
                    fromDate: fromDate.Value,
                    toDate: toDate == null ? fromDate.Value : toDate.Value,
                    moduleCode: a.ModuleCode).ToList();

                for (int i = 0; i < batchTable.Count(); i++)
                {
                    query += $@"SELECT * FROM {batchTable[i]}";

                    if (i < batchTable.Count() - 1)
                        query += " UNION ALL ";
                }

                query = $@"SELECT * FROM ({query}) 
                    [tbl] WHERE {filter.ToSQL().Replace("[]", "[tbl]")}";

                return DAO.Query<BTableEntity>(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetBatchTableName(string yc, string moduleCode)
        {
            try
            {
                return DAO.GetBatchTableName(yc: yc, moduleCode: moduleCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetBatchNo(string yc, string moduleCode)
        {
            try
            {
                return DAO.GetBatchNo(yc: yc, moduleCode: moduleCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override IEnumerable<BTableEntity> Finds(Expression<Func<BTableEntity, bool>> filter)
        {
            try
            {
                return this.Finds(selector: null, filter: filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Delete(Expression<Func<BTableEntity, bool>> filter, string table = "")
        {
            try
            {
                DAO.Delete(filter: filter, table: table);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<JournalSummaryDTO> GetSummary(Expression<Func<JournalSummaryDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetSummary(where: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetOpeningBalance(int date, string accsk)
        {
            try
            {
                return DAO.GetOpeningBalance(date: date, accsk: accsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void Validate(IEnumerable<BTableEntity> data)
        {
            try
            {
                if (data.Count() == 0)
                {
                    throw new Exception("'Account' data should not be empty.");
                }

                var postDate = data.First().PostDate;

                var moduleCode = data.First().ModuleCode;
              
                if (data.Where(x => x.PostDate == 0).Count() > 0)
                {
                    throw new Exception("'Post Date' should not be empty.");
                }

                if (!_batchModule.IsValidEntry(postDate.Value, moduleCode))
                {
                    throw new Exception($"'{postDate}' is invalid date for current financial year.");
                }

                if (data.Where(x => string.IsNullOrWhiteSpace(x.AccSK)).Count() > 0)
                {
                    throw new Exception("'Áccount Code' should not be empty.");
                }

                if (data.Where(x => x.CURRATE == 0).Count() > 0)
                {
                    throw new Exception("'CURRATE' should be greater than zero.");
                }

                if (data.Where(x => string.IsNullOrWhiteSpace(x.CURRSK)).Count() > 0)
                {
                    throw new Exception("'CURRSK' should not be empty.");
                }

                if (data.Where(x => string.IsNullOrWhiteSpace(x.CURROP)).Count() > 0)
                {
                    throw new Exception("'CURROP' should not be empty.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void BindData(ref IEnumerable<BTableEntity> data,
            string moduleCode,
            int? postDate,
            TransType transType,
            string parentId = null,
            bool requireToGenerateId = false,
            int recordStatus = (int)RecordStatus.Save)
        {
            try
            {
                Guard.IsNotNullOrEmpty(moduleCode, "moduleCode");
                Guard.IsNotNull(postDate, "postDate");

                if (string.IsNullOrEmpty(parentId))
                    parentId = GeneralUtility.Key;
              
                var period = _accountingPeriod.Finds(x => x.Date1 <= postDate && x.Date2 >= postDate).Single();
                var batch = _batchControl.Finds(x => x.YC == period.YC && x.ModuleCode == moduleCode).Single();

                var setting = new RecordIndexrContext();

                if (requireToGenerateId && data.First().R1 == CommonConstant.DefaultIndexing.R1)
                    setting = _recordIndexer.Generate(TransType.Journal, data.First());

                data.Each(d =>
                {
                    d.SysKey = GeneralUtility.Key;
                    if (requireToGenerateId && d.R1 == CommonConstant.DefaultIndexing.R1)
                    {
                        d.R1 = setting.R1;
                        d.Postfix = setting.Postfix;
                        d.Prefix = setting.Prefix;
                        d.VoucherId = setting.VoucherId;
                    }

                    d.RecordStatus = recordStatus;
                    d.ParentId = parentId;
                    d.PostDate = postDate;
                    d.ModuleCode = moduleCode;
                    d.YCPC = $"{period.YC}{period.PC}";
                    d.BatchNo = batch.SysKey;
                    d.TransType = (int)transType;
                    d.TransTypeDescription = transType.ToDescription();
                    d.SetTableName(batch.BatchTableName);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void UpdateTransaction(IEnumerable<BTableEntity> data,
           string moduleCode,
           string parentId,
           int? postDate,
           TransType transType,
           RecordStatus status)
        {
            try
            {
                Guard.IsNotNullOrEmpty(moduleCode, "moduleCode");
                Guard.IsNotNullOrEmpty(parentId, "parentId");
                Guard.IsNotNull(postDate, "postDate");

                Validate(data);

                BindData(data: ref data,
                    moduleCode: moduleCode,
                    postDate: postDate,
                    transType: transType,
                    parentId: parentId,
                    requireToGenerateId: false,
                    recordStatus: (int)status);
              
                var batchno = data.First().BatchNo;
                var batch = _batchControl.Finds(filter: f => f.SysKey == batchno).Single();

                Delete(filter: f => f.ParentId == parentId,
                    data.First().GetMetadata().TableName);

                data.Each(d =>
                {
                    //make sure time stamp should not changed.
                    d.CreatedDate = data.First().CreatedDate;
                    d.CreatedTime = data.First().CreatedTime;
                  
                    Insert(d);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
