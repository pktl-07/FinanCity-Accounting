namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    public class CommissionPayableAccountResult : IBTableResult<CommissionPaymentEntity>
    {
        private CommissionPaymentEntity _entity;
        private SalesInvoiceEntity _invoice;
        private TransType _transType;
        private string _secondRef;
        private AllocationEntity _allocationEntity;
        private PropertyTransactionDetailEntity _salesOrderDetailEntity;
        private TaxEntity _expenseTax;
        private CommissionAllocation _commissionAllocation;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private Tax _tax;
        private SalesInvoice _salesInvoice;
        private Person _person;
        private IEssAppRepository _app;
        private CashbookSetup _cashbook;

        public CommissionPayableAccountResult(CommissionAllocation allocation,
            PropertyTransactionDetail propertyTransactionDetail,
            CashbookSetup cashbook,
            Tax tax,
            IEssAppRepository app,
            SalesInvoice salesInvoice,
            Person person)
        {
            _app = app;
            _cashbook = cashbook;
            _commissionAllocation = allocation;
            _propertyTransactionDetail = propertyTransactionDetail;
            _tax = tax;
            _person = person;
            _salesInvoice = salesInvoice;
        }

        public override IEnumerable<BTableEntity> Convert(IEnumerable<CommissionPaymentEntity> entityList,
            object parameters)
        {
            var bTableList = new List<BTableEntity>();

            _entity = entityList.ToList().First();
            _allocationEntity = _commissionAllocation.Finds(filter: f => f.SysKey == _entity.ALOSK).Single();
            _salesOrderDetailEntity = _propertyTransactionDetail.Finds(filter: f => f.SysKey == _allocationEntity.DetailSK).Single();
            _expenseTax = _tax.Finds(filter: f => f.SysKey == _salesOrderDetailEntity.TaxSK).SingleOrDefault() ?? new TaxEntity();
            _invoice = _salesInvoice.Finds(filter: f => f.SysKey == _allocationEntity.HeaderSK).Single();
            _transType = (TransType)_person.Finds(filter: f => f.SysKey == _allocationEntity.SPSK).Single().TransType;
            _secondRef = $"{_invoice.R1}, {_entity.R1}";

            if (_transType == TransType.InternalSalesperson)
            {
                bTableList.Add(SalespersonPayableAccount());
                //bTableList.AddRange(DebtListAccount());
                bTableList.Add(BankAccount());
            }
            else
            {
                bTableList.Add(CoBrokePayableAccount());
                bTableList.Add(BankAccount());
            }

            var diff = bTableList.Sum(x => x.TransactionAmount.ToRoundUp());
            if (diff != 0)
            {
                bTableList.Add(new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.DecimalAdjustment.Key,
                    PostDate = _entity.DatePaid,
                    SecondRef = _secondRef,
                    LineDescription = _entity.R1,
                    TransactionAmount = decimal.Negate(diff),
                    EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
                    ParentId = _invoice.SysKey,
                    SourceHeaderTransSK = _entity.SysKey,
                    TransType = (int)TransType.CommissionPayable,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                    CURRATE = _entity.CURRATE,
                    CURROP = _entity.CURROP,
                    CURRSK = _entity.CURRSK
                });
            }

            return bTableList;
        }

        //private BTableEntity PurchaseTaxAccount
        //{
        //    get
        //    {
        //        Func<decimal, decimal, decimal> taxAmount = (gross, gst) =>
        //        {
        //            var total = gross + ((gross / 100) * gst);

        //            return (((total / ((100 + gst) / 100)) / 100) * gst);
        //        };

        //        return new BTableEntity
        //        {
        //            AccSK = _expenseTax.InputTaxSK,
        //            PostDate = _entity.DatePaid,
        //            SecondRef = _secondRef,
        //            LineDescription = _entity.R1,
        //            TransactionAmount = taxAmount(_allocationEntity.AmountBeforeGST, (decimal)_expenseTax.TaxPercent),
        //            ParentId = _invoice.SysKey,

        //            //TaxAmount = _expenseTax.TaxPercent > 0 ? _allocation.GSTValue : GetCoBrokerPayableAmount(),
        //            TaxAmount = ConvertTaxAmount(sign: '+',
        //            taxAmt: GetCoBrokerPayableAmount(),
        //            taxSK: _expenseTax.SysKey),

        //            TaxSK = _expenseTax.SysKey,
        //            TaxPercent = (decimal)_expenseTax.TaxPercent,
        //            EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
        //            SourceHeaderTransSK = _entity.SysKey,
        //            TransType = (int)TransType.CommissionPayable,
        //            RecordStatus = (int)RecordStatus.Post,
        //            ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
        //            CURRATE = _entity.CURRATE,
        //            CURROP = _entity.CURROP,
        //            CURRSK = _entity.CURRSK
        //        };
        //    }
        //}
        private BTableEntity SalespersonPayableAccount()
        {
            return new BTableEntity
            {
                AccSK = _app.DefaultAccountingCode.SalespersonAccurals.Key,
                PostDate = _entity.DatePaid,
                SecondRef = _secondRef,
                LineDescription = _entity.R1,
                TransactionAmount = GetCoBrokerPayableAmount(),
                EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
                ParentId = _invoice.SysKey,
                SourceHeaderTransSK = _entity.SysKey,
                TransType = (int)TransType.CommissionPayable,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                CURRATE = _entity.CURRATE,
                CURROP = _entity.CURROP,
                CURRSK = _entity.CURRSK
            };
        }

        private BTableEntity CoBrokePayableAccount()
        {
            return new BTableEntity
            {
                AccSK = _app.DefaultAccountingCode.CobrokerAccurals.Key,
                PostDate = _entity.DatePaid,
                SecondRef = _secondRef,
                LineDescription = _entity.R1,
                EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
                TransactionAmount = GetCoBrokerPayableAmount(),
                ParentId = _invoice.SysKey,
                SourceHeaderTransSK = _entity.SysKey,
                TransType = (int)TransType.CommissionPayable,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                TaxSK = _expenseTax.TaxPercent == 0 ? _expenseTax.SysKey : null,
                TaxAmount = _expenseTax.TaxPercent == 0 ? GetCoBrokerPayableAmount() : 0,
                CURRATE = _entity.CURRATE,
                CURROP = _entity.CURROP,
                CURRSK = _entity.CURRSK
            };
        }

        private BTableEntity BankAccount()
        {
            return new BTableEntity
            {
                AccSK = _cashbook.Finds(filter: f => f.SysKey == _entity.CBSK).Single().AccSK,
                PostDate = _entity.DatePaid,
                SecondRef = _secondRef,
                LineDescription = _entity.R1,
                EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
                TransactionAmount = decimal.Negate(_entity.NetPaid),
                ParentId = _invoice.SysKey,
                SourceHeaderTransSK = _entity.SysKey,
                TransType = (int)TransType.CommissionPayable,
                RecordStatus = (int)RecordStatus.Post,
                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                CURRATE = _entity.CURRATE,
                CURROP = _entity.CURROP,
                CURRSK = _entity.CURRSK
            };
        }

        //private List<BTableEntity> DebtListAccount()
        //{
        //    var debtAccount = new List<BTableEntity>();

        //    foreach (var debt in new DebtReceivedDetail { Entity = new DebtReceivedDetailEntity { ParentSK = _entity.SysKey, RecordStatus = _entity.RecordStatus } }.Finds())
        //    {
        //        debtAccount.Add(new BTableEntity
        //        {
        //            AccSK = new DebtDescription().FindBySysKey(debt.DebtSK).IncomeAccountSK,
        //            PostDate = _entity.DatePaid,
        //            SecondRef = _secondRef,
        //            LineDescription = _entity.R1,
        //            EntryDescription = TransType.CommissionPayable.ToTaxDescription(),
        //            TransactionAmount = decimal.Negate(debt.ReceivedAmount),
        //            ParentID = _invoice.SysKey,
        //            SourceHeaderTransSK = _entity.SysKey,
        //            TransType = (int)TransType.CommissionPayable,
        //            RecordStatus = (int)RecordStatus.Post,
        //            ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
        //            CURRATE = _entity.CURRATE,
        //            CURROP = _entity.CURROP,
        //            CURRSK = _entity.CURRSK
        //        });
        //    }
        //    return debtAccount;
        //}

        private decimal GetCoBrokerPayableAmount()
        {
            if (_transType == TransType.ExternalSalesPerson)
            {
                return _allocationEntity.AmountBeforeGST;
            }
            else
            {
                return _entity.NetPaid;
            }
        }
    }
}
