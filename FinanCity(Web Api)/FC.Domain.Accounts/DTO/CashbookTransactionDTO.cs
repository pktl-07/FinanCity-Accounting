namespace FC.Domain.Accounts.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class CashbookTransactionDTO : QueryAssert<CashbookTransactionDTO>
    {
        public CashbookTransactionDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.ParentId).Custom("tbl");
            RuleFor(x => x.ReferenceNo).Custom("tbl");
            RuleFor(x => x.TransactionDate).Custom("tbl");
            RuleFor(x => x.CashbookName).Custom("tbl");
            RuleFor(x => x.CashbookDescription).Custom("tbl");
            RuleFor(x => x.TransDescription).Custom("tbl");
            RuleFor(x => x.TotalNetAmount).Custom("tbl");
            RuleFor(x => x.RecordStatus).Custom("tbl");
            RuleFor(x => x.ManualRef).Custom("tbl");
            RuleFor(x => x.ChequeNo).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
            RuleFor(x => x.RefTransType).Custom("tbl");
            RuleFor(x => x.SecondRef).Custom("tbl");
            RuleFor(x => x.CashbookType).Custom("tbl");
            RuleFor(x => x.JRef).Custom("tbl");
            RuleFor(x => x.CURRSK).Custom("tbl");
            RuleFor(x => x.CURRATE).Custom("tbl");
            RuleFor(x => x.CURROP).Custom("tbl");
            RuleFor(x => x.CurrencyCode).Custom("tbl");
        }

        public string SysKey { get; set; }

        public string ParentId { get; set; }

        public string ReferenceNo { get; set; }

        public int? TransactionDate { get; set; }

        public string CashbookName { get; set; }

        public string CashbookDescription { get; set; }

        public string TransDescription { get; set; }

        public decimal TotalNetAmount { get; set; }

        public int RecordStatus { get; set; }

        public string ManualRef { get; set; }

        public string ChequeNo { get; set; }

        public int TransType { get; set; }

        public int RefTransType { get; set; }

        public string SecondRef { get; set; }

        public string CashbookType { get; set; }

        public string JRef { get; set; }

        public string CURRSK { get; set; }

        public string CURROP { get; set; }

        public decimal CURRATE { get; set; }

        public string CurrencyCode { get; set; }
    }
}
