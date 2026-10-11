namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Accounts.Infrastructure;

    public class CashBookTransaction : BusinessLogic<CashBookTransactionEntity, CashBookTransactionDAO>
    {
        #region Fields

        private RecordIndexerFactory _recordIndexer;
        private IBTableResult<CashBookTransactionEntity> _bTableResult;
     
        #endregion

        #region Ctor

        public CashBookTransaction(RecordIndexerFactory recordIndexer,
            CashbookTransactionAccountResult bTableResult)
        {
            _bTableResult = bTableResult;
            _recordIndexer = recordIndexer;
        }

        #endregion

        #region Methods

        public IDataPagerResult<CashbookTransactionDTO> GetPager(int currentPage,
             int pageSize,
             Expression<Func<CashbookTransactionDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string InsertAsTransaction(IEnumerable<CashBookTransactionEntity> insertedItems)
        {
            try
            {
                var key = this.GetParentId();

                insertedItems.Each(a =>
                {
                    a.ParentId = key;
                    a.RecordStatus = (int)RecordStatus.Save;

                    Insert(a);
                });

                return key;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string UpdateAsTransaction(IEnumerable<CashBookTransactionEntity> insertedItems,
            IEnumerable<CashBookTransactionEntity> updatedItems,
            IEnumerable<CashBookTransactionEntity> deletedItems)
        {
            try
            {
                var key = this.GetParentId(updatedItems: updatedItems, deletedItems: deletedItems);

                insertedItems.Each(a =>
                {
                    a.ParentId = key;
                    Insert(a);
                });

                updatedItems.Each(a =>
                {
                    a.ParentId = key;
                    Update(selector => a, filter: f => f.SysKey == a.SysKey);
                });

                deletedItems.Each(a =>
                {
                    a.ParentId = key;
                    Delete(filter: f => f.SysKey == a.SysKey);
                });

                return key;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string PostAsTransaction(IEnumerable<CashBookTransactionEntity> insertedItems,
            IEnumerable<CashBookTransactionEntity> updatedItems,
            IEnumerable<CashBookTransactionEntity> deletedItems,
            bool accounting = true)
        {
            try
            {
                var setting = _recordIndexer.Generate(
                   insertedItems.Count() > 0 ? (TransType)insertedItems.First().TransType : (TransType)updatedItems.First().TransType,
                   insertedItems.Count() > 0 ? insertedItems.First() : updatedItems.First());

                insertedItems.Each(a =>
                {
                    a.R1 = setting.R1;
                    a.Prefix = setting.Prefix;
                    a.Postfix = setting.Postfix;
                    a.VoucherId = setting.VoucherId;
                });

                updatedItems.Each(a =>
                {
                    a.R1 = setting.R1;
                    a.Prefix = setting.Prefix;
                    a.Postfix = setting.Postfix;
                    a.VoucherId = setting.VoucherId;
                });

                var key = this.UpdateAsTransaction(insertedItems: insertedItems,
                      updatedItems: updatedItems,
                      deletedItems: deletedItems);

                if (accounting)
                {
                    var cbt = Finds(filter: f => f.ParentId == key);

                    var btableEntityList = _bTableResult.Convert(entityList: cbt,
                       parameters: cbt.ToList().First());

                    _bTableResult.OnJournalPosted += (sender, e) =>
                    {
                        Update(selector: s => new CashBookTransactionEntity
                        {
                            RecordStatus = (int)RecordStatus.Post,
                            JRef = e.JournalNo
                        }, filter: f => f.ParentId == key);
                    };

                    _bTableResult.ExcutePost(btableEntityList);
                }

                return key;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReverseAsTransaction(string parentId)
        {
            try
            {
                var info = Finds(filter: f => f.ParentId == parentId).First();
                var module = (info.TransType == (int)TransType.CashBookReceived) ? CommonConstant.Module.MODULE_CASHBOOK_RECEIPT : CommonConstant.Module.MODULE_CASHBOOK_PAYMENT;

                Update(selector: s => new CashBookTransactionEntity { RecordStatus = (int)RecordStatus.Update },
                    filter: f => f.ParentId == parentId);

                _bTableResult.ExcuteReverse(key: info.ParentId,
                    postDate: info.TransactionDate.Value,
                    module: module);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void VoidAsTransaction(string parentId)
        {
            try
            {
                var info = Finds(filter: f => f.ParentId == parentId).First();
                var module = (info.TransType == (int)TransType.CashBookReceived) ? CommonConstant.Module.MODULE_CASHBOOK_RECEIPT : CommonConstant.Module.MODULE_CASHBOOK_PAYMENT;

                Update(selector: s => new CashBookTransactionEntity { RecordStatus = (int)RecordStatus.Void },
                    filter: f => f.ParentId == parentId);

                _bTableResult.ExcuteVoid(key: info.ParentId,
                    postDate: info.TransactionDate.Value,
                    module: module);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<BTableEntity> RecapAsTransaction(string parentId,
            int transType)
        {
            try
            {
                var cbt = Finds(filter: f => f.ParentId == parentId
                && f.TransType == transType);

                return _bTableResult.Convert(entityList: cbt,
                    parameters: cbt.ToList().First());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        public string GetParentId(
           IEnumerable<CashBookTransactionEntity> updatedItems = null,
           IEnumerable<CashBookTransactionEntity> deletedItems = null)
        {
            try
            {
                if (updatedItems != null)
                {
                    var u = updatedItems.Where(x => !string.IsNullOrWhiteSpace(x.ParentId)).FirstOrDefault();
                    if (u != null)
                    {
                        return u.ParentId;
                    }
                }
                if (deletedItems != null)
                {
                    var d = deletedItems.Where(x => !string.IsNullOrWhiteSpace(x.ParentId)).FirstOrDefault();
                    if (d != null)
                    {
                        return d.ParentId;
                    }
                }
                return GeneralUtility.Key;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

    }
}
