namespace FC.Domain.Accounts.Entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FC.Core;
    using FC.Core.Data;

    [Description("JOB")]
    [Table(Name = "JOB")]
    [RecordIndexer(AutoGenerateIdReferencesOnSave = true)]
    public class JobEntity : EntityBase
        , IUniqueFieldConstraint<JobEntity>
        , IReferencesConstraint
    {
        public JobEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "Name")]
        public string Name
        {
            get;
            set;
        }

        [Column(Name = "Description"), Display(Name = "Job Description")]
        public string Description
        {
            get;
            set;
        }

        [Column(Name = "Contact")]
        public string Contact
        {
            get;
            set;
        }

        [Column(Name = "IsActive")]
        public bool IsActive
        {
            get;
            set;
        }

        [Column(Name = "StartDate", IsDate = true)]
        public int? StartDate
        {
            get;
            set;
        }

        [Column(Name = "FinishDate", IsDate = true)]
        public int? FinishDate
        {
            get;
            set;
        }

        [Column(Name = "Manager")]
        public string Manager
        {
            get;
            set;
        }

        [Column(Name = "JobCreatedDate", IsDate = true)]
        public string JobCreatedDate
        {
            get;
            set;
        }

        public void UniqueField(IUniqueFieldAnalyzer<JobEntity> action)
        {
            action.Register(x => new object[] { x.Description });
        }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<CashBookTransactionEntity>(x => x.JobSK);
        }
    }
}
