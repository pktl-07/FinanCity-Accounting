namespace FC.Domain.Version.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "Version")]
    public partial class VersionEntity : EntityBase
    {
        public VersionEntity() : base("FC.Biz") { }

        [Column(Name = "VersionNo")]
        public string VersionNo { get; set; }
    }
}
