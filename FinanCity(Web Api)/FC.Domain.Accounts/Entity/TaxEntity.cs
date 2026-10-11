namespace FC.Domain.Accounts.Entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FC.Core;
    using FC.Core.Data;
    
    [Table(Name = "ACC008")]
    [Description("Tax")]
    public class TaxEntity : EntityBase 
        , IUniqueFieldConstraint<TaxEntity>
        , IReferencesConstraint
    {
        public TaxEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "TaxCode")]
        public string TaxCode { get; set; }

        [Column(Name = "Description"), Display(Name = "Tax Name")]
        public string Description { get; set; }

        [Column(Name = "TaxPercent")]
        public decimal TaxPercent { get; set; }

        [Column(Name = "InputTaxSK"), Display(Name = "Input Account Code")]
        public string InputTaxSK { get; set; }

        [Column(Name = "OutputTaxSK"), Display(Name = "Output Account Code")]
        public string OutputTaxSK { get; set; }

        [Column(Name = "TaxType")]
        public int TaxType { get; set; }

        [Column(Name = "TaxCategory"), Display(Name = "Tax Category")]
        public int TaxCategory { get; set; }

        [Column(Name = "IsInclusive")]
        public bool IsInclusive { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        [Column(Name = "IsSpecialImportScheme")]
        public bool IsSpecialImportScheme { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "Remark")]
        public string Remark { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<TaxEntity> action)
        {
            action.Register(f => new object[] { f.TaxCode });
        }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<CashBookTransactionEntity>(x => x.TaxSK);
        }
    }
}
