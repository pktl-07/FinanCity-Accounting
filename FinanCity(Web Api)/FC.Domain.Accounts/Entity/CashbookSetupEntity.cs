namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "CBM001")]
    public class CashbookSetupEntity : EntityBase
        , IUniqueFieldConstraint<CashbookSetupEntity>
        , IReferencesConstraint
    {
        public CashbookSetupEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "CashbookName"), Display(Name = "CashbookName")]
        public string CashbookName { get; set; }

        [Column(Name = "CBType"), Display(Name = "Cashbook Type")]
        public int CBType { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "Remark")]
        public string Remark { get; set; }

        [Column(Name = "AccSK"), Display(Name = "Account Code")]
        public string AccSK { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        [Column(Name = "BankAccountNo")]
        public string BankAccountNo { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "OpeningBalance")]
        public decimal OpeningBalance { get; set; }

        [Column(Name = "UnReconcileOpeningBalance")]
        public decimal UnReconcileOpeningBalance { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "ForeignExchangeGainSK")]
        public string ForeignExchangeGainSK { get; set; }

        [Column(Name = "ForeignExchangeLossSK")]
        public string ForeignExchangeLossSK { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<CashbookSetupEntity> action)
        {
            action.Register(f => new object[] { f.CashbookName });
        }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<CashBookTransactionEntity>(x => x.CBSK);
        }
    }
}
