namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using Ninject;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class CommissionHelperFactory
    {
        private Person _person;
        private PropertyTransactionDetail _salesOrderDetail;
        private CommissionStructureDetail _commissionScheme;
        private IEssAppRepository _app;

        public CommissionHelperFactory(Person person,
            IEssAppRepository app,
            CommissionStructureDetail commissionScheme,
            PropertyTransactionDetail salesOrderDetail)
        {
            _app = app;
            _commissionScheme = commissionScheme;
            _person = person;
            _salesOrderDetail = salesOrderDetail;
        }

        public CommissionContext GetContext(string invoiceSK,
           string saleOrderDetailSK,
           string spsk,
           decimal allocatedAmt,
           string alosk,
           string batchNo,
           decimal grossToPromoteNext)
        {
            var person = _person.Finds(filter: f => f.SysKey == spsk).Single();
          
            if (person.TransType == (int)FC.Core.TransType.ExternalSalesPerson)
            {
                return DI.Current.Get<ExternalContext>().GetContext(
                    invoiceSK: invoiceSK,
                    saleOrderDetailSK: saleOrderDetailSK,
                    allocatedAmt: allocatedAmt,
                    spsk: spsk,
                    alosk: alosk,
                    batchNo: batchNo,
                    grossToPromoteNextLevel: grossToPromoteNext);
            }
           
            var sod = _salesOrderDetail.Finds(filter: f => f.SysKey == saleOrderDetailSK).Single();
            var scheme = _commissionScheme.Finds(filter: f => f.SysKey == sod.SchemeDetailSK).Single();

            if (scheme.Type == (int)FC.Core.CommissonStructureType.Tier
                && _app.EnableSalespersonTierBonus)
            {
                return DI.Current.Get<TireContext>().GetContext(
                    invoiceSK: invoiceSK,
                    saleOrderDetailSK: saleOrderDetailSK,
                    allocatedAmt: allocatedAmt,
                    spsk: spsk,
                    alosk: alosk,
                    batchNo: batchNo,
                    grossToPromoteNextLevel: grossToPromoteNext);
            }

            return DI.Current.Get<FixedContext>().GetContext(
                   invoiceSK: invoiceSK,
                   saleOrderDetailSK: saleOrderDetailSK,
                   allocatedAmt: allocatedAmt,
                   spsk: spsk,
                   alosk: alosk,
                   batchNo: batchNo,
                   grossToPromoteNextLevel: grossToPromoteNext);
        }
    }
}
