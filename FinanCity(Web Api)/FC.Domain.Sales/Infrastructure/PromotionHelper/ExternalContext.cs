namespace FC.Domain.Sales.Infrastructure
{
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    public class ExternalContext : BaseCommissionHelper
    {

        #region Ctor

        public ExternalContext(Person person,
         Employment employment,
         OverridingScheme overridingScheme,
         CommissionStructureDetail commissionStructureDetail,
         OverridingProfile overridingProfile,
         PropertyTransactionDetail salesOrderDetail)
         : base(person,
               employment,
               overridingScheme,
               commissionStructureDetail,
               overridingProfile,
               salesOrderDetail)
        {
            
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
                 grossToPromoteNextLevel : 0);

            var amount = allocatedAmt.ToRoundUp();

            result.AMS104List = new PayableSchemeEntity[]
            {
                new PayableSchemeEntity
                {
                    SysKey = GeneralUtility.Key,
                    RecordStatus = (int)RecordStatus.Post,
                    ALOSK = alosk,
                    GrossComm = amount,
                    AgencyPercent = 0.00M,
                    AssociatePercent = 100,
                    AgencyComm = 0,
                    AssociateComm = amount,
                    SPSK = SPSK,
                    InvoiceSK = invoiceSK,
                    //JUN003
                    OverrideSchemeProfileSK = null,
                    //JUN004
                    OverrideSchemeSK = null,
                }
            };

            return result;
        }

        #endregion
    }
}
