namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "PostalCode")]
    public class PostalCodeEntity : EntityBase
    {
        public PostalCodeEntity() : base("FC.Biz") { }

        [Column(Name = "AddressType")]
        public string AddressType { get; set; }

        [Column(Name = "BuildingNumber")]
        public string BuildingNumber { get; set; }

        [Column(Name = "StreetSK")]
        public string StreetSK { get; set; }

        [Column(Name = "BuildingSK")]
        public string BuildingSK { get; set; }
    }
}
