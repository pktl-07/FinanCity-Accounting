namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
  
    public class FixedContext : BaseCommissionHelper
    {
        #region Fields

        private PropertyTransactionDetail _salesOrderDetail;
        private OverridingProfile _overridingProfile;
        private OverridingScheme _overridingScheme;

        #endregion

        #region Ctor

        public FixedContext(Person person,
         PropertyTransactionDetail saleOrderDetail,
         Employment employment,
         OverridingScheme overridingScheme,
         CommissionStructureDetail commissionStructureDetail,
         OverridingProfile overridingProfile)
         : base(person,
               employment,
               overridingScheme,
               commissionStructureDetail,
               overridingProfile,
               saleOrderDetail)
        {
            _salesOrderDetail = saleOrderDetail;
            _overridingProfile = overridingProfile;
            _overridingScheme = overridingScheme;
        }

        #endregion

        #region Methods

        public override CommissionContext GetContext(string invoiceSK,
            string saleOrderDetailSK,
            string spsk,
            decimal allocatedAmt,
            string alosk,
            string batchNo,
            decimal grossToPromoteNextLevel)
        {
            var result = base.GetContext(invoiceSK,
                 saleOrderDetailSK,
                 spsk,
                 allocatedAmt,
                 alosk,
                 batchNo,
                 grossToPromoteNextLevel);
            
            var sod = _salesOrderDetail.Finds(filter: f => f.SysKey == saleOrderDetailSK).Single();
            var current = SchemeDetailEnties.Where(x => x.SchemeSK == sod.SchemeDetailSK).Single();
            var amount = allocatedAmt.ToRoundUp();
            var bonus = new List<BonusPayableEntity>();
        
            result.AMS104List = new PayableSchemeEntity[]
            {
                new PayableSchemeEntity
                {
                    SysKey = GeneralUtility.Key,
                    RecordStatus = (int)RecordStatus.Post,
                    ALOSK = ALOSK,
                    GrossComm = amount,
                    AgencyPercent = 100 - sod.CurrentCommission , // current.AgencyPercent ?? 0.00M,
                    AssociatePercent = sod.CurrentCommission,  // current.AssociatedPercent ?? 0.00M,
                    AgencyComm = ((amount / 100) * (100 - sod.CurrentCommission)).ToRoundUp(),
                    AssociateComm = ((amount / 100) * sod.CurrentCommission).ToRoundUp(),
                    SPSK = SPSK,
                    InvoiceSK = InvoiceSK,
                    //JUN003
                    OverrideSchemeProfileSK =
                    _overridingProfile.Finds(filter: f => f.SPSK == spsk && f.PTOSK == invoiceSK).Single().SysKey,
                    //JUN004
                    OverrideSchemeSK = 
                    _overridingScheme.Finds(filter: f => f.SchemeSK == current.SchemeSK && f.BatchNo == batchNo).Single().SysKey
                }
            };

            BuildBonusScheme(result.AMS104List, bonus);

            result.AMS105List = bonus;

            return result;
        }

        #endregion
    }
}
