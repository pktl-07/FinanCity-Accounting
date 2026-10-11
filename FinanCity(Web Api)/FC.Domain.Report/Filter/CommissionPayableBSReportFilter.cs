namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionPayableBSReportFilter : QueryAssert<CommissionPayableBSReportFilter>
        , ISalespersonFor
        , ISaleDateFor
        , ISalespersonTypeFor
        , ICobrokerAccuralChangeDateFor
    {
        public CommissionPayableBSReportFilter() : base(CommonConstant.SalesConnection)
        {

            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.SPSK).Custom();
            RuleFor(x => x.SalespersonType).Custom();
            RuleFor(x => x.CobrokerAccuralChangeDate).Custom();
        }

        public int? SalesDate { get; set; }

        public string SPSK { get; set; }

        public int SalespersonType { get; set; }

        public int? CobrokerAccuralChangeDate { get; set; }
    }
}
