namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class IndividualSalesReportFilter : QueryAssert<IndividualSalesReportFilter>
        , ISalespersonFor
        , ICommissionReceivedStatusFor
        , ICommissionPaidStatusFor
        , ISaleDateFor
        , IFranchiseeFor
    {
        public IndividualSalesReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.ReceivedStatus).Custom();
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.PaidStatus).Custom();
            RuleFor(x => x.FranchiseeSK).Custom();
        }

        public string SPSK { get; set; }

        public int ReceivedStatus { get; set; }

        public int? SalesDate { get; set; }

        public int PaidStatus { get; set; }

        public string FranchiseeSK { get; set; }
    }
}
