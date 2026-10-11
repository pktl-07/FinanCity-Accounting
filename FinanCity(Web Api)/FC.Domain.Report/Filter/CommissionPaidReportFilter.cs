namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionPaidReportFilter : QueryAssert<CommissionPaidReportFilter>
        , ISalespersonFor
        , IPaidDate
        , ISalespersonTypeFor
        , ICommissionPaidStatusFor
    {
        public CommissionPaidReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.SalespersonType).Custom();
            RuleFor(x => x.PaidDate).Custom();
            RuleFor(x => x.PaidStatus).Custom();
        }

        public string SPSK { get; set; }

        public int SalespersonType { get; set; }

        public int PaidStatus { get; set; }

        public int? PaidDate { get; set; }
    }
}
