namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class SalespersonCommissionDTO : QueryAssert<SalespersonCommissionDTO>
    {
        #region Ctor

        public SalespersonCommissionDTO() : base("FC.Biz")
        {
            RuleFor(x => x.PropertySK).Custom("tbl");
            RuleFor(x => x.TransactionNo).Custom("tbl");
            RuleFor(x => x.TransactionDate).Custom("tbl");
            RuleFor(x => x.PropertyAddress).Custom("tbl");
            RuleFor(x => x.LoanAmount).Custom("tbl");
            RuleFor(x => x.Bank).Custom("tbl");
            RuleFor(x => x.GrossComm).Custom("tbl");
            RuleFor(x => x.ClientName).Custom("tbl");
            RuleFor(x => x.Status).Custom("tbl");
            RuleFor(x => x.SPSK).Custom("tbl");
            RuleFor(x => x.MarketingExpense).Custom("tbl");
            RuleFor(x => x.AgencyAmount).Custom("tbl");
            RuleFor(x => x.PayableAmount).Custom("tbl");
            RuleFor(x => x.GrossAmount).Custom("tbl");
            RuleFor(x => x.AmountPaid).Custom("tbl");
        }

        #endregion

        #region Properties

        public string PropertySK { get; set; }
        
        public string TransactionNo { get; set; }
        
        public string TransactionDate { get; set; }
        
        public string PropertyAddress { get; set; }
        
        public decimal LoanAmount { get; set; }

        public decimal GrossAmount { get; set; }
        
        public decimal MarketingExpense { get; set; }
        
        public decimal AgencyAmount { get; set; }

        public decimal AmountPaid { get; set; }
        
        public decimal PayableAmount { get; set; }
        
        public string Bank { get; set; }
        
        public decimal GrossComm { get; set; }
        
        public string ClientName { get; set; }
        
        public string SPSK { get; set; }

        public int Status { get; set; }

        #endregion
    }
}
