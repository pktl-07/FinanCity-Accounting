namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class SalesBonusReportFilter : QueryAssert<SalesBonusReportFilter>
        , IT1
        , ISaleDateFor
        , IRankFor
        , IFranchiseeFor
    {
        public SalesBonusReportFilter() : base(CommonConstant.SalesConnection)
        {
            RuleFor(x => x.T1).Custom();
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.Rank).Custom();
            RuleFor(x => x.FranchiseeSK).Custom();
        }
        public string T1 { get; set; }

        public int? SalesDate { get; set; }

        public int Rank { get; set; }
     
        public string FranchiseeSK { get; set; }
    }
}
