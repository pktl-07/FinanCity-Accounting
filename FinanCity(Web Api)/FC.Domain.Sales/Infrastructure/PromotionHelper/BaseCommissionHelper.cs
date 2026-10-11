namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Core.Infrastructure;
    using Ninject;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
   
    public abstract class BaseCommissionHelper
    {
        #region Fields

        public string InvoiceSK { get; set; }
        
        public string SODSK { get; set; }
        
        public string SPSK { get; set; }
        
        public decimal AllocateAmount { get; set; }
        
        public string ALOSK { get; set; }
        
        public string BatchNo { get; set; }
       
        public Person _person;
        
        private Employment _employment;
       
        public IEnumerable<OverridingSchemeEntity> SchemeDetailEnties;
        
        private OverridingScheme _overridingScheme;
        
        private CommissionStructureDetail _commissionStructureDetail;
        
        private OverridingProfile _overridingProfile;

        private PropertyTransactionDetail _salesOrderDetail;


        private IEssAppRepository _app
        {
            get { return DI.Current.Get<IEssAppRepository>(); }
        }

        #endregion

        #region Ctor
        public BaseCommissionHelper() { }

        #endregion

        #region Methods

        public BaseCommissionHelper(Person person,
            Employment employment,
            OverridingScheme overridingScheme,
            CommissionStructureDetail commissionStructureDetail,
            OverridingProfile overridingProfile,
            PropertyTransactionDetail salesOrderDetail)
        {
            _person = person;
            _employment = employment;
            _overridingScheme = overridingScheme;
            _commissionStructureDetail = commissionStructureDetail;
            _overridingProfile = overridingProfile;
            _salesOrderDetail = salesOrderDetail;
        }

        public virtual CommissionContext GetContext(string invoiceSK,
            string saleOrderDetailSK,
            string spsk,
            decimal allocatedAmt,
            string alosk,
            string batchNo,
            decimal grossToPromoteNextLevel)
        {
            Guard.IsNotNullOrEmpty(invoiceSK, "invoiceSK");
            Guard.IsNotNullOrEmpty(saleOrderDetailSK, "saleOrderDetailSK");
            Guard.IsNotNullOrEmpty(spsk, "spsk");
            Guard.IsNotNegative(allocatedAmt, "allocatedAmt");
            Guard.IsNotNullOrEmpty(alosk, "alosk");
            Guard.IsNotNullOrEmpty(batchNo, "batchNo");

            this.InvoiceSK = invoiceSK;
            this.SODSK = saleOrderDetailSK;
            this.SPSK = spsk;
            this.AllocateAmount = allocatedAmt;
            this.ALOSK = alosk;
            this.BatchNo = batchNo;

            var result = new CommissionContext();

            result.SalespersonEntity = _person.Finds(filter: f => f.SysKey == spsk).Single();

            switch (result.SalespersonEntity.TransType)
            {
                case (int)TransType.InternalSalesperson:

                    result.EmploymentEntity = _employment.Finds(filter: f => f.SysKey == spsk).Single();

                    //the comment out beflow has a purpose that it cause error after commission
                    //scheme has changed for salesperson and it does not reflect correctly
                    //due to it's old HeaderSK.
                    var sod = _salesOrderDetail.Finds(filter: f => f.SysKey == saleOrderDetailSK).Single();
                    var headerSK = _commissionStructureDetail.Finds(filter: f => f.SysKey == sod.SchemeDetailSK).Single().ParentSK;
                    
                    SchemeDetailEnties = _overridingScheme.Finds(filter: f => f.BatchNo == batchNo
                    && f.SchemeHeaderSK == headerSK);

                    break;
            }

            return result;
        }

        public void BuildBonusScheme(IEnumerable<PayableSchemeEntity> payableEntities,
            IList<BonusPayableEntity> bonusPayableEntities)
        {
            if (_app.EnableSalespersonTierBonus)
            {
                foreach (var payableScheme in payableEntities)
                {
                    if (_person.Finds(filter: f => f.SysKey == payableScheme.SPSK).Single().TransType == (int)TransType.InternalSalesperson)
                    {
                        var overrideScheme = _overridingScheme.
                           Finds(filter: f => f.SysKey == payableScheme.OverrideSchemeSK).Single();

                        var overrideProfile = _overridingProfile.Finds(filter: f => f.SysKey == payableScheme.OverrideSchemeProfileSK).Single();
                        var tireRankList = _overridingProfile.GetTireRank(overrideProfile, overrideScheme);

                        foreach (var tireRank in tireRankList)
                        {
                            if (!string.IsNullOrEmpty(tireRank.TireSK))
                            {
                                if (!tireRank.TireSK.Equals(SalepersonStatus.Dummy.ToDescription(), StringComparison.OrdinalIgnoreCase))
                                {
                                    bonusPayableEntities.Add(new BonusPayableEntity
                                    {
                                        PayableSchemeSK = payableScheme.SysKey,
                                        Percentage = tireRank.Percentage,
                                        Rank = tireRank.Rank,
                                        SPSK = tireRank.TireSK,
                                        Tier = tireRank.TireId,
                                        TransType = (int)TransType.OverridingBonusPayable,
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
