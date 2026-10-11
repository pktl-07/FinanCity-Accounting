namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using Ninject;
   
    [Table(Name = "JUN003")]
    public class OverridingProfileEntity : EntityBase
    {
        public OverridingProfileEntity() : base("FC.Biz") { }

        [Column(Name = "PTOSK")]
        public string PTOSK { get; set; }

        [Column(Name = "SPSK")]
        public string SPSK { get; set; }

        [Column(Name = "L1SK")]
        public string L1SK { get; set; }

        [Column(Name = "L2SK")]
        public string L2SK { get; set; }

        [Column(Name = "L3SK")]
        public string L3SK { get; set; }

        [Column(Name = "L4SK")]
        public string L4SK { get; set; }

        [Column(Name = "L5SK")]
        public string L5SK { get; set; }

        [Column(Name = "L1Rank")]
        public int L1Rank { get; set; }

        [Column(Name = "L2Rank")]
        public int L2Rank { get; set; }

        [Column(Name = "L3Rank")]
        public int L3Rank { get; set; }

        [Column(Name = "L4Rank")]
        public int L4Rank { get; set; }

        [Column(Name = "L5Rank")]
        public int L5Rank { get; set; }

        [Column(Name = "SPRank")]
        public int SPRank { get; set; }

        [Column(Name = "UpTo")]
        public string UpTo { get; set; }

        [Column(Name = "OpeningAccSales")]
        public decimal OpeningAccSales { get; set; }

        [Column(Name = "Rank")]
        public int Rank { get; set; }

        [Column(Name = "ResetCommissionEffectiveStartDate", IsDate = true)]
        public int? ResetCommissionEffectiveStartDate { get; set; }

        [Column(Name = "MGRSK")]
        public string MGRSK { get; set; }
       
        [Column(Name = "DDSK")]
        public string DDSK { get; set; }
      
        [Column(Name = "MGRPayoutPercent")]
        public decimal MGRPayoutPercent { get; set; }
       
        [Column(Name = "DDPayoutPercent")]
        public decimal DDPayoutPercent { get; set; }
       
    }
}
