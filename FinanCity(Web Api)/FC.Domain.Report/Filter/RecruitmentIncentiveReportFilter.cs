namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class RecruitmentIncentiveReportFilter : QueryAssert<RecruitmentIncentiveReportFilter>
        , ISalespersonFor
        , ISaleDateFor
    {
        public RecruitmentIncentiveReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.SalesDate).Custom();
        }

        public string SPSK { get; set; }

        public int? SalesDate { get; set; }
    }
}
