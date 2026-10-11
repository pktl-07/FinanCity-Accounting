namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CashbookLogReportFilter : QueryAssert<CashbookLogReportFilter>
       , ISaleDateFor
       , ICashbookFor
    {
        public CashbookLogReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.CBSK).Custom();
        }
        
        public int? SalesDate { get; set; }
     
        public string CBSK { get; set; }
    }
}
