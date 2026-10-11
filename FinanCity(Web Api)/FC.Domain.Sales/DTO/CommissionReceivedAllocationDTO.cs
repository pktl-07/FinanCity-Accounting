namespace FC.Domain.Sales.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionReceivedAllocationDTO : QueryAssert<CommissionReceivedAllocationDTO>
    {
        #region Ctor

        public CommissionReceivedAllocationDTO() : base("FC.Biz")
        {
            RuleFor(x => x.ActualAmt).Custom("tbl");
            RuleFor(x => x.DebitAmt).Custom("tbl");
            RuleFor(x => x.CreditAmt).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.InvoiceDate).Custom("tbl");
            RuleFor(x => x.SecondRef).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.TotalReceivedAmt).Custom("tbl");
            RuleFor(x => x.TotalAllocatedAmt).Custom("tbl");
            RuleFor(x => x.SalesStatus).Custom("tbl");
            RuleFor(x => x.TransactionNo).Custom("tbl");
        }

        #endregion

        #region Properties

        public decimal ActualAmt { get; set; }
        
        public decimal DebitAmt { get; set; }
        
        public decimal CreditAmt { get; set; }
        
        public string TransactionNo { get; set; }
        
        public string InvoiceNo { get; set; }
        
        public string InvoiceDate { get; set; }
        
        public string SecondRef { get; set; }
        
        public string ClientName { get; set; }
        
        public string Address { get; set; }
        
        public string Street { get; set; }
        
        public string InvoiceSK { get; set; }
        
        public decimal TotalReceivedAmt { get; set; }
        
        public decimal TotalAllocatedAmt { get; set; }
     
        public int SalesStatus { get; set; }
        
        public decimal Balance
        {
            get { return TotalReceivedAmt.ToRoundUp() - TotalAllocatedAmt.ToRoundUp(); }
        }

        #endregion
    }
}
