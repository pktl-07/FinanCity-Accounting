namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "TNC_PropertyType")]
    public class TNC_PropertyTypeEntity : EntityBase,
        IReferencesConstraint
    {
        public TNC_PropertyTypeEntity() : base("FC.Biz") { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "IncomeAccountSK"), Display(Name = "Income Account")]
        public string IncomeAccountSK { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<TNC_Entity>(x => x.PropertyTypeSK);
        }      
    }
}
