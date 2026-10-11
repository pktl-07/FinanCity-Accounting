namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class CashbookReceiptPaymentReportFilter : QueryAssert<CashbookReceiptPaymentReportFilter>
       , ISaleDateFor
       , ICashbookFor
       , ITransTypeFor
    {
        public CashbookReceiptPaymentReportFilter() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.CBSK).Custom();
            RuleFor(x => x.TransType).Custom();
        }

        public int? SalesDate { get; set; }

        public string CBSK { get; set; }

        public int? TransType { get; set; }
    }
}
