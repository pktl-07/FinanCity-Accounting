namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "CBM004")]
    [RecordIndexer(AutoGenerateIdReferencesOnPost = true)]
    public class CashBookTransferEntity : EntityBase
    {
        public CashBookTransferEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "FromCBSysKey"), Display(Name = "From Cashbook")]
        public string FromCBSysKey { get; set; }

        [Column(Name = "SecondRef")]
        public string SecondRef { get; set; }

        [Column(Name = "JRef")]
        public string JRef { get; set; }

        [Column(Name = "TransactionDate", IsDate = true), Display(Name = "Transaction Date")]
        public int? TransactionDate { get; set; }

        [Column(Name = "ChequeNo")]
        public string ChequeNo { get; set; }

        [Column(Name = "ToCBSysKey"), Display(Name = "To Cashbook")]
        public string ToCBSysKey { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "FromTransactionAmount"), Display(Name = "From Transaction Amount")]
        public decimal FromTransactionAmount { get; set; }

        [Column(Name = "ToTransactionAmount"), Display(Name = "To Transaction Amount")]
        public decimal ToTransactionAmount { get; set; }

        [Column(Name = "FROMCURRSK")]
        public string FROMCURRSK { get; set; }

        [Column(Name = "FROMCURRATE")]
        public decimal FROMCURRATE { get; set; }

        [Column(Name = "FROMCURROP")]
        public string FROMCURROP { get; set; }

        [Column(Name = "TOCURRSK")]
        public string TOCURRSK { get; set; }

        [Column(Name = "TOCURRATE")]
        public decimal TOCURRATE { get; set; }

        [Column(Name = "TOCURROP")]
        public string TOCURROP { get; set; }
    }
}
