namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class AgedReceivableFilter : QueryAssert<AgedReceivableFilter>
        , IClientFor
        , ISaleDateFor
    {
        public AgedReceivableFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.ClientSK).Custom();
        }

        public int? SalesDate { get; set; }

        public string ClientSK { get; set; }
    }
}
