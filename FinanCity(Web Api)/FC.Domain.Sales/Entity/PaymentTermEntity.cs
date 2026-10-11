namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
   
    [Table(Name = "PaymentTerm")]
    public partial class PaymentTermEntity : EntityBase
    {
        public PaymentTermEntity() : base("FC.Biz") { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "Day")]
        public int Day { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}
