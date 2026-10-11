namespace FC.Domain.Report.Filter
{
    using FC.Core.Data;

    public class CommissionReceivedReportDTO : QueryAssert<CommissionReceivedReportDTO>
    {
        public CommissionReceivedReportDTO() : base("FC.Biz")
        {
            RuleFor(x => x.ReceiptNo).Custom("tbl");
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.ReceiptDate).Custom("tbl");
            RuleFor(x => x.Inv_VoucherId).Custom("tbl");
            RuleFor(x => x.TransNo).Custom("tbl");
            RuleFor(x => x.PropertyContact).Custom("tbl");
            RuleFor(x => x.InvoiceAmt).Custom("tbl");
            RuleFor(x => x.ReceivedAmt).Custom("tbl");
            RuleFor(x => x.Balance).Custom("tbl");
            RuleFor(x => x.Cashbook).Custom("tbl");
            RuleFor(x => x.ChequeNo).Custom("tbl");
            RuleFor(x => x.CBSK).Custom("tbl");
            RuleFor(x => x.SecondRef).Custom("tbl");
            RuleFor(x => x.CR_Prfix).Custom("tbl");
            RuleFor(x => x.CR_VoucherID).Custom("tbl");
            RuleFor(x => x.Client).Custom("tbl");
            RuleFor(x => x.Bank).Custom("tbl");
        }

        public string ReceiptNo { get; set; }

        public string InvoiceSK { get; set; }

        public string InvoiceNo { get; set; }

        public string ReceiptDate { get; set; }

        public int Inv_VoucherId { get; set; }

        public string TransNo { get; set; }

        public string PropertyContact { get; set; }

        public decimal InvoiceAmt { get; set; }

        public decimal ReceivedAmt { get; set; }

        public decimal Balance { get; set; }

        public int Auto { get; set; }

        public string ReceivedMode { get; set; }

        public string Cashbook { get; set; }

        public string ChequeNo { get; set; }

        public string CBSK { get; set; }

        public string SecondRef { get; set; }

        public string CR_Prfix { get; set; }

        public int CR_VoucherID { get; set; }

        public string Bank { get; set; }

        public string Client { get; set; }

    }
}
