namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using Ninject;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    public class CommissionAllocationAccountResult : IBTableResult<AllocationEntity>
    {
        #region Fields

        private SalesInvoiceEntity _salesInvoiceEntity;
        private decimal _promotionExpense = decimal.Zero;
        private decimal _teamManagerIncome = decimal.Zero;
        private IEnumerable<PayableSchemeEntity> _schemeList;
        private AllocationEntity _entity;
        private IEssAppRepository _app;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private OverridingScheme _overridingScheme;
        private PayableScheme _payableScheme;
        private PropertyTransaction _propertyTransaction;

        private SalesInvoice _salesInvoice
        {
            get { return DI.Current.Get<SalesInvoice>(); }
        }

        #endregion

        #region Ctor

        public CommissionAllocationAccountResult(PayableScheme payableScheme,
            IEssAppRepository app,
            PropertyTransaction propertyTransaction,
            PropertyTransactionDetail propertyTransactionDetail,
            OverridingScheme overridingScheme)
        {
            _app = app;
            _propertyTransaction = propertyTransaction;
            _propertyTransactionDetail = propertyTransactionDetail;
            _overridingScheme = overridingScheme;
            _payableScheme = payableScheme;
        }

        #endregion

        #region Methods

        public override IEnumerable<BTableEntity> Convert(IEnumerable<AllocationEntity> entities,
        object parameters)
        {
            var result = new List<BTableEntity>();

            _promotionExpense = decimal.Zero; // need to intialize default
            _teamManagerIncome = decimal.Zero; // need to intialize default
            _entity = entities.ToList().First();
            _schemeList = _payableScheme.Finds(filter: f => f.ALOSK == _entity.SysKey);
            _salesInvoiceEntity = _salesInvoice.Finds(filter: f => f.SysKey == _entity.HeaderSK).Single();

            var transaction = _propertyTransaction.Finds(filter: f => f.SysKey == _entity.HeaderSK).Single();

            PoppulatePromotionExpense();

            result.Add(PromotionExpense);
            // result.Add(TeamManagerAccurals);
            // result.Add(OverridingIncome);
            result.Add(SalespersonAccurals);

            var diff = result.Sum(x => x.TransactionAmount.ToRoundUp());
            if (diff != 0)
            {
                result.Add(new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.DecimalAdjustment.Key,
                    PostDate = _entity.AllocationDate,
                    SecondRef = _entity.R1,
                    LineDescription = _entity.R1,
                    EntryDescription = TransType.AllocationCommission.ToTaxDescription(),
                    TransactionAmount = decimal.Negate(diff),
                    ParentId = _entity.SysKey,
                    SourceHeaderTransSK = _entity.SysKey,
                    TransType = (int)TransType.AllocationCommission,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_SALE,
                    CURRSK = _app.CURRSK,
                    CURRATE = _app.CURRATE,
                    CURROP = _app.CURROP
                });
            }

            return result.Where(x => x.TransactionAmount != 0);
        }

        #endregion

        #region Utilities

        private BTableEntity PromotionExpense
        {
            get
            {
                return new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.PromotionalExpense.Key,
                    PostDate = _entity.AllocationDate,
                    SecondRef = _entity.R1,
                    LineDescription = _entity.R1,
                    EntryDescription = TransType.AllocationCommission.ToTaxDescription(),
                    TransactionAmount = _promotionExpense,
                    ParentId = _entity.SysKey,
                    SourceHeaderTransSK = _entity.SysKey,
                    TransType = (int)TransType.AllocationCommission,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_SALE,
                    CURRSK = _app.CURRSK,
                    CURRATE = _app.CURRATE,
                    CURROP = _app.CURROP
                };
            }
        }

        //private BTableEntity TeamManagerAccurals
        //{
        //    get
        //    {
        //        return new BTableEntity
        //        {
        //            AccSK = _app.DefaultAccountingCode.TeamManagerAccurals.Key,
        //            PostDate = _entity.AllocationDate,
        //            SecondRef = _entity.R1,
        //            LineDescription = _entity.R1,
        //            TransactionAmount = _teamManagerIncome,
        //            EntryDescription = TransType.AllocationCommission.ToTaxDescription(),
        //            ParentId = _entity.SysKey,
        //            SourceHeaderTransSK = _entity.SysKey,
        //            TransType = (int)TransType.AllocationCommission,
        //            RecordStatus = (int)RecordStatus.Post,
        //            ModuleCode = CommonConstant.Module.MODULE_SALE,
        //            CURRSK = _app.CURRSK,
        //            CURRATE = _app.CURRATE,
        //            CURROP = _app.CURROP,
        //        };
        //    }
        //}

        //private BTableEntity OverridingIncome
        //{
        //    get
        //    {
        //        return new BTableEntity
        //        {
        //            AccSK = _app.DefaultAccountingCode.OverridingIncome.Key,
        //            PostDate = _entity.AllocationDate,
        //            SecondRef = _entity.R1,
        //            LineDescription = _entity.R1,
        //            EntryDescription = TransType.AllocationCommission.ToTaxDescription(),
        //            TransactionAmount = decimal.Negate(_teamManagerIncome),
        //            ParentId = _entity.SysKey,
        //            SourceHeaderTransSK = _entity.SysKey,
        //            TransType = (int)TransType.AllocationCommission,
        //            RecordStatus = (int)RecordStatus.Post,
        //            ModuleCode = CommonConstant.Module.MODULE_SALE,
        //            CURRSK = _app.CURRSK,
        //            CURRATE = _app.CURRATE,
        //            CURROP = _app.CURROP
        //        };
        //    }
        //}

        private BTableEntity SalespersonAccurals
        {
            get
            {
                return new BTableEntity
                {
                    AccSK = _app.DefaultAccountingCode.SalespersonAccurals.Key,
                    PostDate = _entity.AllocationDate,
                    SecondRef = _entity.R1,
                    LineDescription = _entity.R1,
                    EntryDescription = TransType.AllocationCommission.ToTaxDescription(),
                    TransactionAmount = decimal.Negate(_promotionExpense),
                    ParentId = _entity.SysKey,
                    SourceHeaderTransSK = _entity.SysKey,
                    TransType = (int)TransType.AllocationCommission,
                    RecordStatus = (int)RecordStatus.Post,
                    ModuleCode = CommonConstant.Module.MODULE_SALE,
                    CURRSK = _app.CURRSK,
                    CURRATE = _app.CURRATE,
                    CURROP = _app.CURROP
                };
            }
        }

        private void PoppulatePromotionExpense()
        {
            Func<decimal, decimal, decimal> percentValue = (x, y) =>
            {
                return (y / 100) * x;
            };

            foreach (var scheme in _schemeList)
            {
                var saleDetail = _propertyTransactionDetail.Finds(filter: f => f.SysKey == _entity.DetailSK).Single();
                var invoiceComission = saleDetail.CurrentCommission;
                var currentScheme = _overridingScheme.Finds(filter: f => f.SysKey == scheme.OverrideSchemeSK).Single().SchemeSK;

                #region Promotion 

                if (invoiceComission != scheme.AssociatePercent)
                {
                    if (scheme.AssociatePercent > invoiceComission)
                        _promotionExpense += percentValue(scheme.GrossComm.ToRoundUp(), scheme.AssociatePercent).ToRoundUp() -
                            percentValue(scheme.GrossComm.ToRoundUp(), invoiceComission).ToRoundUp();
                    else
                        _promotionExpense -= percentValue(scheme.GrossComm.ToRoundUp(), invoiceComission).ToRoundUp() -
                            percentValue(scheme.GrossComm.ToRoundUp(), scheme.AssociatePercent).ToRoundUp();

                    var invoiceTMPercent = _salesInvoice.Get_TM_BonusPercent(_salesInvoiceEntity.SysKey, scheme.SPSK);
                    var currentTMPercent = _salesInvoice.GetORBonusPercent(_salesInvoiceEntity.SysKey, scheme.SPSK, currentScheme);

                    // TEAM MANAGER
                    if (invoiceTMPercent > currentTMPercent)
                        _teamManagerIncome += percentValue(scheme.GrossComm.ToRoundUp(), invoiceTMPercent).ToRoundUp() -
                            percentValue(scheme.GrossComm.ToRoundUp(), currentTMPercent).ToRoundUp();
                    else
                        _teamManagerIncome -= percentValue(scheme.GrossComm, currentTMPercent).ToRoundUp()
                            - percentValue(scheme.GrossComm, invoiceTMPercent).ToRoundUp();
                }

                #endregion
            }
        }

        #endregion
    }
}
