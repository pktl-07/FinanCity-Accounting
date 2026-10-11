namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class SalesDetailReportFilter : QueryAssert<SalesDetailReportFilter>
        , ICommissionReceivedStatusFor
        , ICommissionPaidStatusFor
        , ISaleDateFor
    {
        public SalesDetailReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.ReceivedStatus).Custom();
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.PaidStatus).Custom();
        }

        public int ReceivedStatus { get; set; }

        public int? SalesDate { get; set; }

        public int PaidStatus { get; set; }
    }
}
