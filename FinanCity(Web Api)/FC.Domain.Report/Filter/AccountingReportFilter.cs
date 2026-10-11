namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class AccountingReportFilter : QueryAssert<AccountingReportFilter>
        , IAccountingDateFor
    {
        public AccountingReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.AccountingDate).Custom();
        }

        public int? AccountingDate { get; set; }
    }
}
