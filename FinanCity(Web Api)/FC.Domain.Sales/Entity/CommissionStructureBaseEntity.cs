namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "CommissionStructureBase")]
    public class CommissionStructureBaseEntity : EntityBase
    {
        public CommissionStructureBaseEntity() : base("FC.Biz") { }

        [Column(Name = "Type")]
        public int Type { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }
    }
}
