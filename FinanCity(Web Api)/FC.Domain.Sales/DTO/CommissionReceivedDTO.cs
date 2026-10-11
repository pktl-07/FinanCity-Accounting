namespace FC.Domain.Sales.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class CommissionReceivedDTO : QueryAssert<CommissionReceivedDTO>
    {
        #region Ctor

        public CommissionReceivedDTO() : base("FC.Biz")
        {
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.InvoiceDate).Custom("tbl");
            RuleFor(x => x.TransactionNo).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.InvoiceAmt).Custom("tbl");
            RuleFor(x => x.TotalReceived).Custom("tbl");
            RuleFor(x => x.SalesStatus).Custom("tbl");
            RuleFor(x => x.Balance).Custom("tbl");
            RuleFor(x => x.InvoiceGST).Custom("tbl");
            RuleFor(x => x.RecordStatus).Custom("tbl");
            RuleFor(x => x.TotalReceivedAmt).Custom("tbl");
            RuleFor(x => x.TotalAllocatedAmt).Custom("tbl");
            RuleFor(x => x.MainClient).Custom("tbl");
            RuleFor(x => x.SecondRef).Custom("tbl");
            RuleFor(x => x.MainSalesperson).Custom("tbl");
        }

        #endregion

        #region Properties

        public string ReceivedSK { get; set; }
        
        public decimal InvoiceGST { get; set; }
        
        public string InvoiceSK { get; set; }
        
        public string InvoiceNo { get; set; }
        
        public string InvoiceDate { get; set; }
        
        public string TransactionNo { get; set; }
        
        public string Address { get; set; }
        
        public decimal InvoiceAmt { get; set; }
        
        public decimal TotalReceived { get; set; }
        
        public int SalesStatus { get; set; }
        
        public int RecordStatus { get; set; }
        
        public decimal TotalReceivedAmt { get; set; }
        
        public string MainClient { get; set; }
        
        public decimal Debit { get; set; }
        
        public decimal Credit { get; set; }
        
        public decimal TotalAllocatedAmt { get; set; }
        
        public decimal Balance
        {
            get
            {
                return InvoiceAmt.ToRoundUp()
                  - Credit.ToRoundUp()
                  + Debit.ToRoundUp()
                  - TotalReceived.ToRoundUp();
            }
        }
        
        public string SecondRef { get; set; }
        
        public string MainSalesperson { get; set; }
        
        #endregion
    }
}
