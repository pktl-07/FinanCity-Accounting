namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "ACC007")]
    public class BatchModuleEntity : EntityBase
    {
        public BatchModuleEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "YC")]
        public string YC
        {
            get;
            set;
        }

        [Column(Name = "PC")]
        public string PC
        {
            get;
            set;
        }

        [Column(Name = "ModuleCode")]
        public string ModuleCode
        {
            get;
            set;
        }

        [Column(Name = "EntryStatus")]
        public int EntryStatus
        {
            get;
            set;
        }
    }
}
