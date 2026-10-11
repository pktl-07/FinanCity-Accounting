namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "CareerManagement")]
    public class CareerManagementEntity : EntityBase
    {
        public CareerManagementEntity() : base("FC.Biz") { }

        [Column]
        public string SPSK { get; set; }

        [Column]
        public int? CreateDate { get; set; }

        [Column]
        public string T1 { get; set; }

        [Column]
        public int? EffectiveDate { get; set; }

        [Column]
        public string ApprovedBy { get; set; }

        [Column]
        public string Remarks { get; set; }

        [Column]
        public int Rank { get; set; }

        [Column]
        public int TransType { get; set; }

        [Column]
        public string RecuriterSK { get; set; }

        [Column]
        public string SchemeDetailSK { get; set; }

        [Column]
        public bool IsActive { get; set; }

        [Column]
        public int ReasonToTerminate { get; set; }

        [Column]
        public bool IsDefault { get; set; }

        [Column]
        public decimal BonusPercent { get; set; }
    }
}
