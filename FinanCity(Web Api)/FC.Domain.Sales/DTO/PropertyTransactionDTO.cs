namespace FC.Domain.Sales.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class PropertyTransactionDTO : QueryAssert<PropertyTransactionDTO> 
        , IMultiCurrency
    {
        #region Ctor

        public PropertyTransactionDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SalesNo).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.PropertySK).Custom("tbl");
            RuleFor(x => x.TransactionDate).Custom("tbl");
            RuleFor(x => x.Exclusive).Custom("tbl");
            RuleFor(x => x.PropertyAddress).Custom("tbl");
            RuleFor(x => x.ClientName).Custom("tbl");
            RuleFor(x => x.SalespersonName).Custom("tbl");
            RuleFor(x => x.CommissionAmount).Custom("tbl");
            RuleFor(x => x.CurrencyCode).Custom("tbl");
            RuleFor(x => x.RecordStatus).Custom("tbl");
            RuleFor(x => x.AUTO).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
            RuleFor(x => x.MortgageBank).Custom("tbl");
            RuleFor(x => x.MainSalespersonSK).Custom("tbl");
            RuleFor(x => x.ClientSK).Custom("tbl");
            RuleFor(x => x.LoanAmount).Custom("tbl");
            RuleFor(x => x.MarketingPromotion).Custom("tbl");
            RuleFor(x => x.InvoiceStatus).Custom("tbl");
        }

        #endregion

        #region Properties

        public string ClientSK { get; set; }   

        public string SalesNo { get; set; }
        
        public string PropertySK { get; set; }
        
        public int? TransactionDate { get; set; }
        
        public bool? Exclusive { get; set; }
        
        public string PropertyAddress { get; set; }

        public string ClientName { get; set; }
        
        public string SalespersonName { get; set; }

        [MultiCurrency]
        public decimal CommissionAmount { get; set; }
        
        public string CurrencyCode { get; set; }
        
        public int RecordStatus { get; set; }
        
        public int AUTO { get; set; }
        
        public int TransType { get; set; }
        
        public string MortgageBank { get; set; }
        
        public string MainSalespersonSK { get; set; }

        [MultiCurrency]
        public decimal LoanAmount { get; set; }

        public string CURRSK { get; set; }
        
        public decimal CURRATE { get; set; }
        
        public string CURROP { get; set; }

        public string InvoiceNo { get; set; }

        public decimal MarketingPromotion { get; set; }

        public int InvoiceStatus { get; set; }

        #endregion
    }
}
