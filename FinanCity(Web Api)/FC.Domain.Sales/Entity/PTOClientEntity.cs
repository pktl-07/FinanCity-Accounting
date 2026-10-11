namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "JUN001")]
    public partial class PTOClientEntity : EntityBase
    {
        public PTOClientEntity() : base("FC.Biz") { }

        [Column(Name = "ClientSK")]
        public string ClientSK { get; set; }

        [Column(Name = "PTOSK")]
        public string PTOSK { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "IsMain")]
        public bool IsMain { get; set; }
    }
}
