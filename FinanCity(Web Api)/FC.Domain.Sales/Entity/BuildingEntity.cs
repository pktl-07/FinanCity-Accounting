namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "Building")]
    public class BuildingEntity : EntityBase
    {
        public BuildingEntity() : base("FC.Biz") { }

        [Column(Name = "BuildingName")]
        public string BuildingName { get; set; }

        [Column(Name = "TypeFlag")]
        public string TypeFlag { get; set; }
    }
}
