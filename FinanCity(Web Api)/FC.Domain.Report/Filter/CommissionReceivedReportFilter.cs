namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionReceivedReportFilter : QueryAssert<CommissionReceivedReportFilter>
        , ISalespersonFor
        , ISaleDateFor
        , ICashbookFor
    {
        public CommissionReceivedReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.CBSK).Custom();
        }

        public int? SalesDate { get; set; }
        public string SPSK { get; set; }
        public string CBSK { get; set; }
    }
}
