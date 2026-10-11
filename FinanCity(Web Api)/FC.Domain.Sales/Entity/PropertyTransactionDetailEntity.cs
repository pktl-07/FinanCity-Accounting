namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS101")]
    public class PropertyTransactionDetailEntity : EntityBase, IMultiCurrency
    {
        public PropertyTransactionDetailEntity() : base("FC.Biz") { }

        [Column]
        public string SPSK { get; set; }

        [Column]
        public string PTOSK { get; set; }

        [Column]
        public bool IsMain { get; set; }

        [Column, MultiCurrency]
        public decimal GrossComm { get; set; }

        [Column, MultiCurrency]
        public decimal GSTValue { get; set; }

        [Column, MultiCurrency]
        public decimal TotalPayable { get; set; }

        [Column]
        public string TaxSK { get; set; }

        private int _transType;
        [Column]
        public int TransType
        {
            get { return _transType; }
            set
            {
                if (IsMain == true)
                    _transType = (int)FC.Core.TransType.MainSalesperson;
                else
                    _transType = value;
            }
        }

        [Column]
        public decimal TaxPercent { get; set; }

        [Column]
        public string DiscountRemarks { get; set; }

        //[Column, MultiCurrency]
        //public decimal Credit { get; set; }

        //[Column, MultiCurrency]
        //public decimal Debit { get; set; }

        [Column]
        public string SchemeDetailSK { get; set; }

        [Column]
        public int ReceivableStatus { get; set; } = (int)SalesViewType.Pending;

        [Column]
        public int PayableStatus { get; set; } = (int)SalesViewType.Pending;

        [Column]
        public int IsCheck { get; set; }

        [Column]
        public decimal CurrentCommission { get; set; }

        //[Column]
        //public decimal ReferralPayout { get; set; }

        [Column]
        public string VendorSK { get; set; }

        [Column]
        public int TenancyNotification { get; set; }

        [Column, MultiCurrency]
        public decimal MarketingExpense { get; set; }

        [Column]
        public string CURRSK { get; set; }

        [Column]
        public decimal CURRATE { get; set; }

        [Column]
        public string CURROP { get; set; }
    }
}
