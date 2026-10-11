namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class TaxReportFilter : QueryAssert<TaxReportFilter>
        , ITaxFor
        , ISaleDateFor
    {
        public TaxReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.TaxSK).Custom();
            RuleFor(x => x.SalesDate).Custom();
        }

        public string TaxSK { get; set; }

        public int? SalesDate { get; set; }
    }
}
