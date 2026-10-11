namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "PayNow")]
    public partial class PayNowEntity : EntityBase
    {
        public PayNowEntity() : base("FC.Biz") { }

        [Column(Name = "MobileNumber"), Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Column(Name = "NRIC")]
        public string NRIC { get; set; }

        [Column(Name = "UEN")]
        public string UEN { get; set; }

        [Column(Name = "ParentSK")]
        public string ParentSK { get; set; }
    }
}
