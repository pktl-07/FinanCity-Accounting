namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class PropertyTransactionListReportFilter : QueryAssert<PropertyTransactionListReportFilter>
        , ISaleDateFor
        , IVendorFor
        , ITNC_PropertyTypeFor
        , ITNC_TransactionTypeFor
        , IRecordStatusFor
    {
        public PropertyTransactionListReportFilter() : base(CommonConstant.SalesConnection)
        {

            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.VendorSK).Custom();
            RuleFor(x => x.TransactionType).Custom();
            RuleFor(x => x.PropertyType).Custom();
            RuleFor(x => x.RecordStatus).Custom();
        }

        public int? SalesDate { get; set; }

        public string VendorSK { get; set; }

        public string PropertyType { get; set; }

        public string TransactionType { get; set; }
     
        public int? RecordStatus { get; set; }
    }
}
