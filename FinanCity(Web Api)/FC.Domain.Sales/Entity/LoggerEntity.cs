namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    
    [Table(Name = "Logger")]
    public partial class LoggerEntity : EntityBase
    {
        public LoggerEntity() : base("FC.Biz") { }

        [Column(Name = "RefSK")]
        public string RefSK { get; set; }

        [Column(Name = "OldValue")]
        public string OldValue { get; set; }

        [Column(Name = "NewValue")]
        public string NewValue { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }
        
        [Column(Name = "TableName")]
        public string TableName { get; set; }

        [Column(Name = "Type")]
        public int Type { get; set; }

        [Column(Name = "CMSK")]
        public string CMSK { get; set; }
    }
}
