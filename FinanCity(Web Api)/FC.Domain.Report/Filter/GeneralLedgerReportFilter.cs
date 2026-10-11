namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;
    using System.Net.Http;

    public class GeneralLedgerReportFilter : QueryAssert<GeneralLedgerReportFilter>
        , ISaleDateFor
        , IAccountCodeFor
    {
        public GeneralLedgerReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.AccountSK).Custom();
        }

        public int? SalesDate { get; set; }

        public string AccountSK { get; set; }
    }

    public class GeneralLedgerAiditReportFilter : QueryAssert<GeneralLedgerAiditReportFilter>
        , ISaleDateFor
        , IAccountCodeFor
        , IGLRef
        , ISecondRef
    {
        public GeneralLedgerAiditReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.AccountSK).Custom();
        }

        public int? SalesDate { get; set; }

        public string AccountSK { get; set; }

        public string SecondRef { get; set; }

        public string GLRef { get; set; }
    }
}
