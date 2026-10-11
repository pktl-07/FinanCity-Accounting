namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Accounts.Infrastructure;

    public class CashBookTransfer : BusinessLogic<CashBookTransferEntity, CashBookTransferDAO>
    {
        #region Fields

        private IBTableResult<CashBookTransferEntity> _bTableResult;
        private CashBookTransaction _cashbookTransaction;
        private AccountSetup _accountSetup;
        private CashbookSetup _cashbookSetup;

        #endregion

        #region Ctor

        public CashBookTransfer(CashbookTransferAccountResult bTableResult,
            CashbookSetup cashbookSetup,
            CashBookTransaction cashbookTransaction,
            AccountSetup accountSetup)
        {
            _cashbookSetup = cashbookSetup;
            _accountSetup = accountSetup;
            _bTableResult = bTableResult;
            _cashbookTransaction = cashbookTransaction;
        }

        #endregion

        #region Methods

        public IDataPagerResult<CashBookTransferEntity> GetPager(
           int currentPage,
           int pageSize,
           Expression<Func<CashBookTransferEntity, bool>> where = null)
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

        public void InsertAsTransaction(ref CashBookTransferEntity entity)
        {
            try
            {
                ConvertMultiCurrency(entity);
                entity = Insert(entity);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateAsTransaction(CashBookTransferEntity entity)
        {
            try
            {
                ConvertMultiCurrency(entity);
                Update(entity, filter: f => f.SysKey == entity.SysKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(CashBookTransferEntity entity)
        {
            try
            {
                ConvertMultiCurrency(entity);

                var fromCashbook = _cashbookSetup.Finds(filter: f => f.SysKey == entity.FromCBSysKey).Single();
                var toCashbook = _cashbookSetup.Finds(filter: f => f.SysKey == entity.ToCBSysKey).Single();

                var btableEntityList = _bTableResult.Convert(entityList: null,
                  parameters: entity);

                _bTableResult.OnJournalPosted += (sender, e) =>
                {
                    #region Cashbook Receipt

                    _cashbookTransaction.PostAsTransaction(insertedItems: new List<CashBookTransactionEntity>
                    {
                         new CashBookTransactionEntity
                         {
                            AccCode = _accountSetup.Finds(filter: f => f.SysKey == toCashbook.AccSK).Single().AccCode,
                            AccSK = toCashbook.AccSK,
                            BankInfo = toCashbook.CBType == (int)CashBookType.Bank 
                            ? toCashbook.Description : "",
                            CBSK = entity.FromCBSysKey,
                            ChequeNo = entity.ChequeNo,
                            TransType = (int)TransType.CashBookPayment,
                            Description = entity.Description,
                            Memo = entity.R1,
                            Inclusive = false,
                            ParentId = GeneralUtility.Key,
                            SourceHeaderTransSK = entity.SysKey,
                            JRef = e.JournalNo,
                            RecordStatus = (int)RecordStatus.Post,
                            Remark = TransType.CashBookTransfer.ToDescription(),
                            SecondRef = entity.R1,
                            TaxAmount = 0,
                            TaxPercent = 0,
                            TaxSK = "1",
                            TotalAmount = entity.ToTransactionAmount,
                            TransactionAmount = entity.ToTransactionAmount,
                            TransactionDate = entity.TransactionDate,
                            RefTransactionType = (int)TransType.CashBookTransfer,
                            CURRATE = entity.FROMCURRATE,
                            CURRSK = entity.FROMCURRSK,
                            CURROP = entity.FROMCURROP
                        }
                    },
                    updatedItems: null,
                    deletedItems: null,
                    accounting : false);

                    _cashbookTransaction.PostAsTransaction(insertedItems: new List<CashBookTransactionEntity>
                    {
                        new CashBookTransactionEntity
                        {
                            AccCode = _accountSetup.Finds(filter: f => f.SysKey == fromCashbook.AccSK).Single().AccCode,
                            AccSK = fromCashbook.AccSK,
                            BankInfo = _cashbookSetup.Finds(filter: f => f.SysKey == entity.FromCBSysKey).Single().CBType == (int)CashBookType.Bank ? fromCashbook.Description : "",
                            CBSK = entity.ToCBSysKey,
                            ChequeNo = entity.ChequeNo,
                            TransType = (int)TransType.CashBookReceived,
                            Inclusive = false,
                            ParentId = GeneralUtility.Key,
                            SourceHeaderTransSK = entity.SysKey,
                            RecordStatus = (int)RecordStatus.Post,
                            JRef = e.JournalNo,
                            Remark = TransType.CashBookTransfer.ToDescription(),
                            Description = entity.Description,
                            Memo = entity.R1,
                            SecondRef = entity.R1,
                            TaxAmount = 0,
                            TaxPercent = 0,
                            TaxSK = "1",
                            TotalAmount = entity.FromTransactionAmount,
                            TransactionAmount = entity.FromTransactionAmount,
                            TransactionDate = entity.TransactionDate,
                            RefTransactionType = (int)TransType.CashBookTransfer,
                            CURRATE = entity.TOCURRATE,
                            CURRSK = entity.TOCURRSK,
                            CURROP = entity.TOCURROP
                        }
                    },
                    updatedItems: null,
                    deletedItems: null,
                    accounting : false);

                    #endregion

                    entity.RecordStatus = (int)RecordStatus.Post;
                    entity.JRef = e.JournalNo;

                    Update(entity, filter: f => f.SysKey == entity.SysKey);
                };

                _bTableResult.ExcutePost(btableEntityList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void VoidAsTransaction(string key)
        {
            try
            {
                var entity = this.Finds(filter: f => f.SysKey == key).Single();
                entity.RecordStatus = (int)RecordStatus.Void;
                Update(entity, filter: f => f.SysKey == key);

                _bTableResult.ExcuteVoid(key: entity.SysKey,
                   postDate: entity.TransactionDate.Value,
                   module: CommonConstant.Module.MODULE_CASHBOOK_PAYMENT);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ReverseAsTransaction(string key)
        {
            try
            {
                var entity = this.Finds(filter: f => f.SysKey == key).Single();

                entity.RecordStatus = (int)RecordStatus.Update;

                Update(entity, filter: f => f.SysKey == key);

                _cashbookTransaction.Update(selector: s => new CashBookTransactionEntity
                {
                    RecordStatus = (int)RecordStatus.Void
                }, filter: f => f.JRef == entity.JRef);

                _bTableResult.ExcuteReverse(key: entity.SysKey,
                   postDate: entity.TransactionDate.Value,
                   module: CommonConstant.Module.MODULE_CASHBOOK_PAYMENT);

                _bTableResult.ExcuteReverse(key: entity.SysKey,
                  postDate: entity.TransactionDate.Value,
                  module: CommonConstant.Module.MODULE_CASHBOOK_RECEIPT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<BTableEntity> RecapAsTransaction(string parentId)
        {
            try
            {
                var cbt = Finds(filter: f => f.SysKey == parentId);

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

        private void ConvertMultiCurrency(CashBookTransferEntity entity)
        {
            if (entity.FROMCURROP == "*")
                entity.FromTransactionAmount = (entity.FromTransactionAmount * entity.FROMCURRATE);
            else
                entity.FromTransactionAmount = (entity.FromTransactionAmount / entity.FROMCURRATE);

            if (entity.TOCURROP == "*")
                entity.ToTransactionAmount = (entity.ToTransactionAmount * entity.TOCURRATE);
            else
                entity.ToTransactionAmount = (entity.ToTransactionAmount / entity.TOCURRATE);
        }

        #endregion
    }
}
