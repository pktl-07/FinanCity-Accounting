namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CompareQuarterlyIncomeStatementReportFilter : QueryAssert<CompareQuarterlyIncomeStatementReportFilter>
        , IYCStartFor
    {
        public CompareQuarterlyIncomeStatementReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.YCStart).Custom();
        }

        public string YCStart { get; set; }
    }
}
