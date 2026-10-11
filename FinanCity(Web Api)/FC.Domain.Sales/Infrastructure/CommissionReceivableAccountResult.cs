namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using Ninject;
    using FC.Core;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
  
    public class CommissionReceivableAccountResult : IBTableResult<CommissionReceivedEntity>
    {
        private CashbookSetup _cashbookSetup = DI.Current.Get<CashbookSetup>();
        private SalesInvoice _salesInvoice = DI.Current.Get<SalesInvoice>();
        private CurrencyTransaction _currencyTransaction = DI.Current.Get<CurrencyTransaction>();
        private IMultiCurrencyConverter _currencyConverter = DI.Current.Get<IMultiCurrencyConverter>();
        private IEssAppRepository _app = DI.Current.Get<IEssAppRepository>();
        private CommissionReceivedEntity _entity;
        private CashbookSetupEntity _cashbookEntity;
        private SalesInvoiceEntity _invoiceEntity;

        private decimal _realizeCurrGainL;
        private string _originRateSK;
        private string _secondRef;
        private decimal _marketingFees;

        public CommissionReceivableAccountResult()
        {
        }

        public override IEnumerable<BTableEntity> Convert(IEnumerable<CommissionReceivedEntity> entityList,
            object parameters)
        {
            _entity = entityList.First();
            _cashbookEntity = _cashbookSetup.Finds(filter: f => f.SysKey == _entity.CBSK).Single();
            _invoiceEntity = _salesInvoice.Finds(filter: f => f.SysKey == _entity.InvoiceSK).Single();
            _originRateSK = OriginCurrSK;
            _secondRef = $"{_invoiceEntity.R1}, {_entity.R1}";
            _marketingFees = (_entity.TotalAmt / _invoiceEntity.ActualAmt) * _marketingFees;

            var originRate = _currencyTransaction.Finds(filter: f => f.ParentSK == _entity.InvoiceSK)
                    .Where(x => x.CURRSK == _originRateSK).First();

            _realizeCurrGainL = RealizeCurrencyGainL(
                total: _currencyConverter.ToMultiCurrency(_entity.TotalAmt, _entity.CURRATE, _entity.CURROP == "/" ? Operator.Divider : Operator.Multiple),
                receiptCurrSK: _entity.CURRSK, receiptRate: _entity.CURRATE, receiptOp: _entity.CURROP,
                invoiceCurrSK: _entity.TransCURRSK, invoiceRate: _entity.TransCURRATE, invoiceOp: _entity.TransCURROP,
                originCurrSK: originRate.CURRSK, originRate: originRate.CURRATE, originOp: originRate.CURROP);

            var bTableList = new List<BTableEntity>();

            bTableList.Add(new BTableEntity
            {
                AccSK = _cashbookEntity.AccSK,
                PostDate = _entity.ReceiptDate,
                SecondRef = _secondRef,
                LineDescription = _entity.R1,
                EntryDescription = TransType.CommissionReceivable.ToTaxDescription(),
                TransactionAmount = _entity.TotalAmt + _realizeCurrGainL,
                ParentId = _entity.InvoiceSK,
                SourceHeaderTransSK = _entity.SysKey,
                TransType = (int)TransType.CommissionReceivable,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                CURRSK = _entity.CURRSK,
                CURRATE = _entity.CURRATE,
                CURROP = _entity.CURROP
            });

            bTableList.Add(new BTableEntity
            {
                AccSK = _app.DefaultAccountingCode.CommisionReceivable.Key,
                PostDate = _entity.ReceiptDate,
                SecondRef = _secondRef,
                LineDescription = _entity.R1,
                EntryDescription = TransType.CommissionReceivable.ToTaxDescription(),
                TransactionAmount = decimal.Negate(_entity.TotalAmt),
                ParentId = _entity.InvoiceSK,
                SourceHeaderTransSK = _entity.SysKey,
                TransType = (int)TransType.CommissionReceivable,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                CURRSK = _entity.CURRSK,
                CURRATE = _entity.CURRATE,
                CURROP = _entity.CURROP
            });

            if (_realizeCurrGainL != 0)
            {
                bTableList.Add(new BTableEntity
                {
                    AccSK = _realizeCurrGainL > 0 ? _cashbookEntity.ForeignExchangeGainSK : _cashbookEntity.ForeignExchangeLossSK,
                    PostDate = _entity.ReceiptDate,
                    SecondRef = _secondRef,
                    LineDescription = _entity.R1,
                    TransactionAmount = decimal.Negate(_realizeCurrGainL),
                    ParentId = _entity.InvoiceSK,
                    SourceHeaderTransSK = _entity.SysKey,
                    TransType = (int)TransType.CommissionReceivable,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                    CURRSK = _entity.CURRSK,
                    CURRATE = _entity.CURRATE,
                    CURROP = _entity.CURROP
                });
            }

            return bTableList.Where(x => x.TransactionAmount != 0);
        }

        private string OriginCurrSK
        {
            get
            {
                if (_entity.TransCURRSK == _app.CURRSK
                     && _entity.CURRSK == _app.CURRSK)
                {
                    return _entity.TransCURRSK;
                }

                if(_entity.TransCURRSK != _app.CURRSK
                    && _entity.CURRSK == _app.CURRSK)
                {
                    return _entity.TransCURRSK;
                }

                if (_entity.TransCURRSK == _app.CURRSK
                    && _entity.CURRSK != _app.CURRSK)
                {
                    return _entity.CURRSK;
                }

                if (_entity.TransCURRSK != _app.CURRSK
                    && _entity.CURRSK != _app.CURRSK)
                {
                    return _entity.TransCURRSK;
                }

                return null;
            }
        }
    }
}
