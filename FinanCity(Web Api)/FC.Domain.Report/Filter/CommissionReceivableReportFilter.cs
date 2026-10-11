namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionReceivableReportFilter : QueryAssert<CommissionReceivableReportFilter>
        , ISalespersonFor
        , ISaleDateFor
        , IClientFor
    {
        public CommissionReceivableReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.SPSK).Custom();
        }

        public int? SalesDate { get; set; }

        public string SPSK { get; set; }
     
        public string ClientSK { get; set; }
    }
}
