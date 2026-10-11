namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CompareMonthlyIncomeStatementFilter : QueryAssert<CompareMonthlyIncomeStatementFilter>
        , IYCStartFor
    {
        public CompareMonthlyIncomeStatementFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.YCStart).Custom();
        }

        public string YCStart { get; set; }
    }
}
