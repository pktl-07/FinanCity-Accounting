namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "CommissionStructure"), Description("Commission Structure")]
    public class CommissionStructureEntity : EntityBase,
        IReferencesConstraint,
        IUniqueFieldConstraint<CommissionStructureEntity>
    {
        public CommissionStructureEntity() : base("FC.Biz") { }

        [Column]
        [Display(Name = "Commission Scheme Name")]
        public string SchemeName { get; set; }

        [Column]
        public int ModuleType { get; set; }

        [Column]
        public int MaxMB { get; set; }

        [Column]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Column]
        public string BaseHeaderSK { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<CommissionStructureDetailEntity>(x => x.ParentSK);
        }

        public void UniqueField(IUniqueFieldAnalyzer<CommissionStructureEntity> action)
        {
            action.Register(f => new object[] { f.SchemeName });
        }
    }
}
