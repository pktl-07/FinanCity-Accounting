namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;
   
    [Description("Department")]
    [Table(Name = "Department")]
    [RecordIndexer(AutoGenerateIdReferencesOnSave = true)]
    public class DepartmentEntity : EntityBase
    {
        public DepartmentEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}
