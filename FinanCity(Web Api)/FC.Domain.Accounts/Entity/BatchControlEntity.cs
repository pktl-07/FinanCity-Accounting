namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
 
    [Table(Name = "ACC005")]
    public class BatchControlEntity : EntityBase
    {
        public BatchControlEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "BatchTableName")]
        public string BatchTableName
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

        [Column(Name = "YC")]
        public string YC
        {
            get;
            set;
        }

        [Column(Name = "Description")]
        public string Description
        {
            get;
            set;
        }
    }
}
