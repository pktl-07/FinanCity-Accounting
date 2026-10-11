namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class SalesSummaryReportFilter : QueryAssert<SalesSummaryReportFilter>
        , ICommissionReceivedStatusFor
        , ISaleDateFor
        , ITNC_TransactionTypeFor
        , ITNC_PropertyTypeFor
        , ICommissionPaidStatusFor
        , IVendorFor
    {
        public SalesSummaryReportFilter() : base(CommonConstant.SalesConnection)
        {

            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.TransactionType).Custom();
            RuleFor(x => x.PropertyType).Custom();
            RuleFor(x => x.ReceivedStatus).Custom();
            RuleFor(x => x.PaidStatus).Custom();
            RuleFor(x => x.VendorSK).Custom();
        }

        public int ReceivedStatus { get; set; }

        public int? SalesDate { get; set; }

        public string TransactionType { get; set; }

        public string PropertyType { get; set; }

        public int PaidStatus { get; set; }
     
        public string VendorSK { get; set; }
    }
}
