namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
  
    public class SalesInvoiceAccountResult 
        : IBTableResult<SalesInvoiceEntity>
    {
        #region Fields

        private string _revenueAccount;
        private TaxEntity _revenueTax;
        private PropertyTransaction _propertyTransaction;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private Tax _tax;
        private IEssAppRepository _app;
        private TNC_PropertyType _propertyType;
        private TNC _tnc;
        private SalesInvoiceEntity _invoiceEntity;
        private IEnumerable<PropertyTransactionDetailEntity> _saleDetailList;
        private PropertyTransactionEntity _propertyTransactionEntity;
        private MarketingExpense _marketingExpense;
        private SalesOrderType _salesOrderType;
        private string _createTime;
        #endregion

        #region Ctor

        public SalesInvoiceAccountResult(TNC tnc,
            TNC_PropertyType propertyType,
            MarketingExpense marketingExpense,
            PropertyTransaction propertyTransaction,
            Tax tax,
            SalesOrderType saleOrderType,
            PropertyTransactionDetail propertyTransactionDetail,
            IEssAppRepository app)
        {
            _salesOrderType = saleOrderType;
            _marketingExpense = marketingExpense;
            _propertyTransaction = propertyTransaction;
            _propertyTransactionDetail = propertyTransactionDetail;
            _tax = tax;
            _app = app;
            _tnc = tnc;
            _propertyType = propertyType;
            _createTime = GeneralUtility.ServerTime;
        }

        #endregion

        #region Methods

        public override IEnumerable<BTableEntity> Convert(IEnumerable<SalesInvoiceEntity> entities,
          object parameters)
        {
            var bTableList = new List<BTableEntity>();

            _invoiceEntity = entities.First();
            _propertyTransactionEntity = _propertyTransaction.Finds(filter: f => f.SysKey == _invoiceEntity.SysKey).Single();
            _revenueTax = _tax.Finds(filter: f => f.SysKey == _invoiceEntity.TAXSK).SingleOrDefault();
            _saleDetailList = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == _invoiceEntity.SysKey);

            if (_propertyTransactionEntity.TransType == (int)TransType.PropertyTransaction)
            {
                var propertyType = _tnc.Finds(filter: f => f.SysKey == _invoiceEntity.SysKey).Single().PropertyTypeSK;
                _revenueAccount = _propertyType.Finds(filter: f => f.SysKey == propertyType).Single().IncomeAccountSK;
            }
            else if (_propertyTransactionEntity.TransType == (int)TransType.ReferralSalesOrder)
            {
                var salesOrderType = _salesOrderType.Finds(filter: f => f.SysKey == _propertyTransactionEntity.SaleOrderTypeSK).Single();
                _revenueAccount = salesOrderType.IncomeAccountSK;
            }

            bTableList.Add(ReceivableAccount);

            foreach (var saleDetail in _saleDetailList)
            {
                if (saleDetail.TransType >= (int)TransType.InternalSalesperson && saleDetail.TransType <= (int)TransType.MainSalesperson)
                {
                    bTableList.Add(new BTableEntity
                        {
                            AccSK = _revenueAccount,
                            PostDate = _invoiceEntity.InvoiceDate,
                            SecondRef = _invoiceEntity.R1,
                            CreatedTime= _createTime,
                            TransactionAmount = _propertyTransactionEntity.TransType == (int)TransType.PropertyTransaction  ? 
                            decimal.Negate(saleDetail.GrossComm.ToRoundUp()) : decimal.Negate(_propertyTransactionEntity.GrossCommission.ToRoundUp()),
                            ParentId = _invoiceEntity.SysKey,
                            SourceHeaderTransSK = _invoiceEntity.SysKey,
                            TransType = (int)TransType.Invoice,
                            RecordStatus = (int)RecordStatus.Post,
                            ModuleCode = CommonConstant.Module.MODULE_SALE,
                            LineDescription = _invoiceEntity.R1,
                            EntryDescription = TransType.Invoice.ToTaxDescription(),
                            CURRSK = _invoiceEntity.CURRSK,
                            CURRATE = _invoiceEntity.CURRATE,
                            CURROP = _invoiceEntity.CURROP
                        });
                  
                    Func<decimal, decimal, decimal, decimal> GetIndividualSalespersonExpense = (grossComm, predicount, currentcommission) =>
                    {
                        return ((grossComm.ToRoundUp() - predicount.ToRoundUp()) / 100) * currentcommission;
                    };
                    //salesperson expense
                    bTableList.Add(new BTableEntity
                    {
                        AccSK = _app.DefaultAccountingCode.CommisionExpense.Key,
                        PostDate = _invoiceEntity.InvoiceDate,
                        SecondRef = _invoiceEntity.R1,
                        CreatedTime = _createTime,
                        TransactionAmount = GetIndividualSalespersonExpense(saleDetail.GrossComm, saleDetail.MarketingExpense, saleDetail.CurrentCommission),
                        ParentId = _invoiceEntity.SysKey,
                        SourceHeaderTransSK = _invoiceEntity.SysKey,
                        TransType = (int)TransType.Invoice,
                        RecordStatus = (int)RecordStatus.Post,
                        LineDescription = _invoiceEntity.R1,
                        EntryDescription = TransType.Invoice.ToTaxDescription(),
                        ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                        CURRSK = _invoiceEntity.CURRSK,
                        CURRATE = _invoiceEntity.CURRATE,
                        CURROP = _invoiceEntity.CURROP
                    });

                    bTableList.Add(new BTableEntity
                    {
                        AccSK = _app.DefaultAccountingCode.SalespersonAccurals.Key,
                        PostDate = _invoiceEntity.InvoiceDate,
                        SecondRef = _invoiceEntity.R1,
                        CreatedTime = _createTime,
                        TransactionAmount = decimal.Negate(GetTotalSPAccuralsWithoutGST(saleDetail.SPSK)),
                        ParentId = _invoiceEntity.SysKey,
                        SourceHeaderTransSK = _invoiceEntity.SysKey,
                        TransType = (int)TransType.Invoice,
                        RecordStatus = (int)RecordStatus.Post,
                        ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                        LineDescription = _invoiceEntity.R1,
                        EntryDescription = TransType.Invoice.ToTaxDescription(),
                        CURRSK = _invoiceEntity.CURRSK,
                        CURRATE = _invoiceEntity.CURRATE,
                        CURROP = _invoiceEntity.CURROP
                    });
                }
                else
                {
                    bTableList.Add(new BTableEntity
                    {
                        AccSK = _revenueAccount,
                        PostDate = _invoiceEntity.InvoiceDate,
                        SecondRef = _invoiceEntity.R1,
                        CreatedTime = _createTime,
                        TransactionAmount = decimal.Negate(saleDetail.GrossComm.ToRoundUp()),
                        ParentId = _invoiceEntity.SysKey,
                        SourceHeaderTransSK = _invoiceEntity.SysKey,
                        TransType = (int)TransType.Invoice,
                        RecordStatus = (int)RecordStatus.Post,
                        LineDescription = _invoiceEntity.R1,
                        EntryDescription = TransType.Invoice.ToTaxDescription(),
                        ModuleCode = CommonConstant.Module.MODULE_SALE,
                        CURRSK = _invoiceEntity.CURRSK,
                        CURRATE = _invoiceEntity.CURRATE,
                        CURROP = _invoiceEntity.CURROP
                    });

                    // salesperson expense
                    bTableList.Add(new BTableEntity
                    {
                        AccSK = _app.DefaultAccountingCode.CommisionExpense.Key,
                        PostDate = _invoiceEntity.InvoiceDate,
                        CreatedTime = _createTime,
                        SecondRef = _invoiceEntity.R1,
                        TransactionAmount = saleDetail.GrossComm.ToRoundUp(),
                        ParentId = _invoiceEntity.SysKey,
                        SourceHeaderTransSK = _invoiceEntity.SysKey,
                        LineDescription = _invoiceEntity.R1,
                        EntryDescription = TransType.Invoice.ToTaxDescription(),
                        TransType = (int)TransType.Invoice,
                        RecordStatus = (int)RecordStatus.Post,
                        ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                        CURRSK = _invoiceEntity.CURRSK,
                        CURRATE = _invoiceEntity.CURRATE,
                        CURROP = _invoiceEntity.CURROP
                    });
                }
            }

            var expense = _marketingExpense.Finds(filter: f => f.PTOSK == _invoiceEntity.SysKey)
                .Sum(x => x.Amount.ToRoundUp());

            if (expense > 0)
            {
                bTableList.Add(new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.MarketingPromotionExpense.Key,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = expense,
                    ParentId = _invoiceEntity.SysKey,
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP,
                });

                bTableList.Add(new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.MarketingPromotionPayable.Key,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = decimal.Negate(expense),
                    ParentId = _invoiceEntity.SysKey,
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP,
                });
            }

            bTableList.Add(CobrokerPayableAccount);

            if (_revenueTax != null)
                bTableList.Add(SaleTaxAccount);

            var diff = bTableList.Sum(x => x.TransactionAmount.ToRoundUp());
            if (diff != 0)
            {
                bTableList.Add(new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.DecimalAdjustment.Key,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = decimal.Negate(diff),
                    ParentId = _invoiceEntity.SysKey,
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP,
                });
            }

            return bTableList.Where(x => x.TransactionAmount != 0);
        }

        #endregion

        #region Accounts

        private BTableEntity ReceivableAccount
        {
            get
            {
                return new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.CommisionReceivable.Key,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = _propertyTransactionEntity.TotalAmount.ToRoundUp(),
                    ParentId = _invoiceEntity.SysKey,
                    TaxAmount = 0,
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    ModuleCode = CommonConstant.Module.MODULE_SALE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP,
                };
            }
        }

        private BTableEntity CobrokerPayableAccount
        {
            get
            {
                return new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.CobrokerAccurals.Key,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = decimal.Negate(GetTotalCBAccuralWithGST()),
                    ParentId = _invoiceEntity.SysKey,
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_PURCHASE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP
                };
            }
        }

        private BTableEntity SaleTaxAccount
        {
            get
            {
                return new BTableEntity
                {
                    AccSK = _revenueTax.OutputTaxSK,
                    PostDate = _invoiceEntity.InvoiceDate,
                    SecondRef = _invoiceEntity.R1,
                    CreatedTime = _createTime,
                    TransactionAmount = decimal.Negate(_propertyTransactionEntity.TaxAmount),
                    ParentId = _invoiceEntity.SysKey,
                    TaxSK = _revenueTax.SysKey,
                    TaxPercent = (decimal)_revenueTax.TaxPercent,

                    TaxAmount = ConvertTaxAmount(sign: '-', taxAmt: _propertyTransactionEntity.GrossCommission.ToRoundUp(),
                    taxSK: _revenueTax.SysKey),
                    
                    LineDescription = _invoiceEntity.R1,
                    EntryDescription = TransType.Invoice.ToTaxDescription(),
                    SourceHeaderTransSK = _invoiceEntity.SysKey,
                    TransType = (int)TransType.Invoice,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_SALE,
                    CURRSK = _invoiceEntity.CURRSK,
                    CURRATE = _invoiceEntity.CURRATE,
                    CURROP = _invoiceEntity.CURROP
                };
            }
        }

        #endregion

        #region Utilities

        private decimal GetTotalSPAccuralsWithoutGST(string spsk)
        {
            var result = decimal.Zero;
            var sd = _saleDetailList.Where(x => x.SPSK == spsk).Single();

            result += (((sd.GrossComm.ToRoundUp() - sd.MarketingExpense.ToRoundUp()) / 100) * sd.CurrentCommission).ToRoundUp();

            return result.ToRoundUp();
        }

        private decimal GetTotalCBAccuralWithGST()
        {
            var result = decimal.Zero;
            foreach (var v in _saleDetailList.Where(x => x.TransType == (int)TransType.ExternalSalesPerson))
            {
                var prediscount = v.GrossComm.ToRoundUp() - v.MarketingExpense.ToRoundUp();
                result += prediscount.ToRoundUp();
            }

            return result;
        }

        #endregion
    }
}
