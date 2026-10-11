namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class CommissionPayableDetailDTO : QueryAssert<CommissionPayableDetailDTO>
    {
        #region Ctor

        public CommissionPayableDetailDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SPSK).Custom("tbl");
            RuleFor(x => x.GSTValue).Custom("tbl");
            RuleFor(x => x.SPName).Custom("tbl");
            RuleFor(x => x.AllocatedAmount).Custom("tbl");
            RuleFor(x => x.AgencyAmount).Custom("tbl");
            RuleFor(x => x.CoBrokerType).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.Agency).Custom("tbl");
            RuleFor(x => x.PayableAmount).Custom("tbl");
            RuleFor(x => x.PaidAmount).Custom("tbl");
            RuleFor(x => x.BalanceAmount).Custom("tbl");
            RuleFor(x => x.SalesOrderSK).Custom("tbl");
            RuleFor(x => x.ALOSK).Custom("tbl");
            RuleFor(x => x.TransactionNo).Custom("tbl");
            RuleFor(x => x.SalespersonType).Custom("tbl");
            RuleFor(x => x.InvoiceNo).Custom("tbl");
            RuleFor(x => x.InvoiceSK).Custom("tbl");
            RuleFor(x => x.GSTPercent).Custom("tbl");
            RuleFor(x => x.MarketingExpense).Custom("tbl");
            RuleFor(x => x.GrossAllocatedAmount).Custom("tbl");
        }

        #endregion

        #region Properties

        public string ALOSK
        {
            get;
            set;
        }

        public decimal MarketingExpense
        {
            get;
            set;
        }

        public string InvoiceSK
        {
            get;
            set;
        }

        public string SPSK
        {
            get;
            set;
        }

        public decimal GSTPercent
        {
            get;
            set;
        }

        public decimal GSTValue
        {
            get;
            set;
        }

        public string SPName
        {
            get;
            set;
        }

        public decimal AllocatedAmount
        {
            get;
            set;
        }

        public decimal GrossAllocatedAmount
        {
            get;
            set;
        }

        public decimal AgencyAmount
        {
            get;
            set;
        }

        public string CoBrokerType
        {
            get;
            set;
        }

        public string NRIC
        {
            get;
            set;
        }

        public string Agency
        {
            get;
            set;
        }

        public decimal PayableAmount
        {
            get;
            set;
        }

        public decimal PaidAmount
        {
            get;
            set;
        }

        public decimal BalanceAmount
        {
            get;
            set;
        }

        public string SalesOrderSK
        {
            get;
            set;
        }

        //public string ALOSK
        //{
        //    get;
        //    set;
        //}

        public string TransactionNo
        {
            get;
            set;
        }

        public string InvoiceNo
        {
            get;
            set;
        }

        public int SalespersonType
        {
            get;
            set;
        }

        #endregion
    }
}
