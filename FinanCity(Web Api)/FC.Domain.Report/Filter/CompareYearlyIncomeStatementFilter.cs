namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CompareYearlyIncomeStatementFilter : QueryAssert<CompareYearlyIncomeStatementFilter>
        , IYCStartFor
        , IYCEndFor
    {
        public CompareYearlyIncomeStatementFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.YCStart).Custom();
            RuleFor(x => x.YCEnd).Custom();
        }

        public string YCStart { get; set; }

        public string YCEnd { get; set; }
    }
}
