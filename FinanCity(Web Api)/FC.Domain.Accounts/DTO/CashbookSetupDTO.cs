namespace FC.Domain.Accounts.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class CashbookSetupDTO : QueryAssert<CashbookSetupDTO>
    {
        #region Ctor

        public CashbookSetupDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.CBSK).Custom("tbl");
            RuleFor(x => x.AccountCode).Custom("tbl");
            RuleFor(x => x.BankAccountName).Custom("tbl");
            RuleFor(x => x.AccountDescription).Custom("tbl");
            RuleFor(x => x.CashbookType).Custom("tbl");
            RuleFor(x => x.CurrencyCode).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.CashbookName).Custom("tbl");
        }

        #endregion

        #region Properties

        public string CBSK { get; set; }
        public string AccountCode { get; set; }
        public string BankAccountName { get; set; }
        public string CashbookName { get; set; }
        public string AccountDescription { get; set; }
        public string CashbookType { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }

        #endregion
    }
}
