namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Qualification")]
    public partial class QualificationEntity : EntityBase,
        IUniqueFieldConstraint<QualificationEntity>,
        IReferencesConstraint
    {
        public QualificationEntity() : base("FC.Biz") { }

        [Column(Name = "Description"), Display(Name = ("Qualification"))]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PersonEducationEntity>(x => x.EduSysKey);
        }

        public void UniqueField(IUniqueFieldAnalyzer<QualificationEntity> action)
        {
            action.Register(x => new object[] { x.Description });
        }
    }
}
