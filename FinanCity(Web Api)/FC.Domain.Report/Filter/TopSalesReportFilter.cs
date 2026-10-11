namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class TopSalesReportFilter : QueryAssert<TopSalesReportFilter>
        , IMaxTopFor
        , ISaleDateFor
    {
        public TopSalesReportFilter() : base(CommonConstant.SalesConnection)
        {

            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.Top).Custom();
        }

        public int? SalesDate { get; set; }
        public int Top { get; set; }
    }
}
