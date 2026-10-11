namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
  
    [Table(Name = "Streets")]
    public class StreetEntity : EntityBase
    {
        public StreetEntity() : base("FC.Biz") { }

        [Column(Name = "StreetName")]
        public string StreetName { get; set; }
    }
}
