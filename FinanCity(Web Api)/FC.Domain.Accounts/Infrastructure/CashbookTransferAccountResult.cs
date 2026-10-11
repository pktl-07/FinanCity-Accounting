namespace FC.Domain.Accounts.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;

    public class CashbookTransferAccountResult : IBTableResult<CashBookTransferEntity>
    {
        #region Fields
       
        private CashbookSetup _cashbookSetup;
        private IEssAppRepository _app;

        #endregion

        #region Ctor

        public CashbookTransferAccountResult(CashbookSetup cashbookSetup,
          IEssAppRepository app)
        {
            _app = app;
            _cashbookSetup = cashbookSetup;
        }

        #endregion

        #region Methods

        public override IEnumerable<BTableEntity> Convert(IEnumerable<CashBookTransferEntity> entityList,
           object parameters)
        {
            var _bTableList = new List<BTableEntity>();
            var entity = (CashBookTransferEntity)parameters;

            _bTableList.Add(new BTableEntity
            {
                AccSK = _cashbookSetup.Finds(filter: f => f.SysKey == entity.ToCBSysKey).Single().AccSK,
                PostDate = entity.TransactionDate,
                SecondRef = entity.R1,
                EntryDescription = entity.Description,
                LineDescription = entity.R1,
                TransactionAmount = entity.FromTransactionAmount,
                IsTaxInclusive = false,
                ParentId = entity.SysKey,
                SourceHeaderTransSK = entity.SysKey,
                TransType = (int)TransType.CashBookTransfer,
                TaxAmount = 0,
                TaxPercent = 0,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                CURRSK = _app.CURRSK,
                CURRATE = _app.CURRATE,
                CURROP = _app.CURROP,
            });

            _bTableList.Add(new BTableEntity
            {
                AccSK = _cashbookSetup.Finds(filter: f => f.SysKey == entity.FromCBSysKey).Single().AccSK,
                PostDate = entity.TransactionDate,
                SecondRef = entity.R1,
                EntryDescription = entity.Description,
                TransactionAmount = decimal.Negate(entity.FromTransactionAmount),
                LineDescription = entity.R1,
                IsTaxInclusive = false,
                ParentId = entity.SysKey,
                SourceHeaderTransSK = entity.SysKey,
                TransType = (int)TransType.CashBookTransfer,
                TaxAmount = 0,
                TaxPercent = 0,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                CURRSK = _app.CURRSK,
                CURRATE = _app.CURRATE,
                CURROP = _app.CURROP,
            });

            return _bTableList;
        }

        #endregion
    }
}
