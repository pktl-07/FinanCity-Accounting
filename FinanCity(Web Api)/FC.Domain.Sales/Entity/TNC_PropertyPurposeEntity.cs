namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "TNC_PropertyPurpose")]
    public class TNC_PropertyPurposeEntity : EntityBase,
        IReferencesConstraint
    {
        public TNC_PropertyPurposeEntity() : base("FC.Biz") { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<TNC_Entity>(x => x.PropertyPurposeSK);
        }
    }
}
