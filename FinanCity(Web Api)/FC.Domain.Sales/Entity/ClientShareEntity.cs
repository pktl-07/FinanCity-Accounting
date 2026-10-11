namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
  
    [Table(Name = "PersonShare")]
    public class ClientShareEntity : EntityBase
    {
        public ClientShareEntity() : base("FC.Biz") { }

        [Column(Name= "RefSK")]
        public string RefSK { get; set; }
    }
}
