namespace FC.Domain.Sales.DTO
{
    using FC.Core;
    using FC.Core.Data;
    
    public class CommissionPayableDTO : QueryAssert<CommissionPayableDTO>
    {
        #region Ctor

        public CommissionPayableDTO() : base("FC.Biz")
        {
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.SalesOrderNo).Custom("tbl");
            RuleFor(x => x.GSTPercent).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.ReceivableAmt).Custom("tbl");
            RuleFor(x => x.TotalPaid).Custom("tbl");
            RuleFor(x => x.SalesStatus).Custom("tbl");
            RuleFor(x => x.TotalPayable).Custom("tbl");
            RuleFor(x => x.AgencyComm).Custom("tbl");
            RuleFor(x => x.ALOSK).Custom("tbl");
            RuleFor(x => x.GrossReceivableAmt).Custom("tbl");
        }

        #endregion

        #region Properties

        public string InvoiceSK { get; set; }
        public string InvoiceNo { get; set; }
        public string SalesOrderNo { get; set; }
        public decimal GSTPercent { get; set; }
        public string Address { get; set; }
        public decimal ReceivableAmt { get; set; }
        public decimal GrossReceivableAmt { get; set; }

        public decimal MarketingExpense { get; set; }
        public decimal GSTValue
        {
            get { return ((ReceivableAmt - (ReceivableAmt / ((100 + GSTPercent) / 100)))).ToRoundUp(); }
        }
        public decimal TotalPaid { get; set; }
        public decimal Balance
        {
            get { return TotalPayable.ToRoundUp() - TotalPaid.ToRoundUp(); }
        }
        public int SalesStatus { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal AgencyComm { get; set; }
        public string ALOSK { get; set; }

        #endregion
    }
}
