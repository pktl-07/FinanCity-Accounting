namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionPayableReportFilter : QueryAssert<CommissionPayableReportFilter>
        , ISalespersonFor
        , ISaleDateFor
        , ISalespersonTypeFor
        , ICommissionPaidStatusFor
        , IPaidDate
    {
        public CommissionPayableReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.SalespersonType).Custom();
            RuleFor(x => x.PaidStatus).Custom();
            RuleFor(x => x.PaidDate).Custom();
        }

        public int? SalesDate { get; set; }

        public string SPSK { get; set; }

        public int SalespersonType { get; set; }

        public int PaidStatus { get; set; }
     
        public int? PaidDate { get; set; }
    }
}
