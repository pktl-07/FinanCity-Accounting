namespace FC.Domain.Sales.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class InvoiceDTO : QueryAssert<InvoiceDTO>
    {
        #region Ctor

        public InvoiceDTO() : base("FC.Biz")
        {
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.JRef).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.RefNo).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.RecordStatus).Custom("tbl");
            RuleFor(x => x.GrossComm).Custom("tbl");
            RuleFor(x => x.TaxPercent).Custom("tbl");
            RuleFor(x => x.TaxValue).Custom("tbl");
            RuleFor(x => x.InvoiceDate).Custom("tbl");
            RuleFor(x => x.ClientName).Custom("tbl");
            RuleFor(x => x.SalespersonName).Custom("tbl");
            RuleFor(x => x.TotalPayable).Custom("tbl");
            RuleFor(x => x.InvoiceAmt).Custom("tbl");
            RuleFor(x => x.CurrencyCode).Custom("tbl");
            RuleFor(x => x.CURRATE).Custom("tbl");
            RuleFor(x => x.CURROP).Custom("tbl");
            RuleFor(x => x.CURRSK).Custom("tbl");
            RuleFor(x => x.MortgageBank).Custom("tbl");
        }

        #endregion

        #region Properties

        public string InvoiceSK { get; set; }

        public string JRef { get; set; }
        
        public string InvoiceNo { get; set; }
        
        public string RefNo { get; set; }
        
        public string Address { get; set; }
        
        public int RecordStatus { get; set; }
        
        [MultiCurrency]
        public decimal GrossComm { get; set; }
        
        public decimal TaxPercent { get; set; }
        
        [MultiCurrency]
        public decimal TaxValue { get; set; }
        
        public int? InvoiceDate { get; set; }
        
        public string ClientName { get; set; }
        
        public string SalespersonName { get; set; }
        
        [MultiCurrency]
        public decimal TotalPayable { get; set; }
        
        [MultiCurrency]
        public decimal InvoiceAmt { get; set; }
        
        public string CurrencyCode { get; set; }
        
        public decimal CURRATE { get; set; }
        
        public string CURROP { get; set; }
        
        public string CURRSK { get; set; }
        
        public string MortgageBank { get; set; }

        #endregion
    }
}
