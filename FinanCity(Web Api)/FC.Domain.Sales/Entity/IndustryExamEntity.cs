namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "IndustryExam"), Description("Industry Exam")]
    public class IndustryExamEntity : EntityBase , 
        IUniqueFieldConstraint<IndustryExamEntity> ,
        IReferencesConstraint
    {
        public IndustryExamEntity() : base("FC.Biz") { }

        [Column(Name = "Description"), Display(Name = ("Industry Exam"))]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PersonIndustryEntity>(x => x.IndusSysKey);
        }

        public void UniqueField(IUniqueFieldAnalyzer<IndustryExamEntity> action)
        {
            action.Register(x => new object[] { x.Description });
        }
    }
}
