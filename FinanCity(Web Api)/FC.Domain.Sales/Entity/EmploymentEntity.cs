namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Employment")]
    public class EmploymentEntity : EntityBase
    {
        public EmploymentEntity() : base("FC.Biz") { }

        [Column(Name = "SPStatusSysKey")]
        public string SPStatusSysKey { get; set; }

        [Column(Name = "OpeningAccSales"), Display(Name = "Opening Acc Sales")]
        public decimal OpeningAccSales { get; set; }

        [Column(Name = "SchemeDetailSysKey")]
        public string SchemeDetailSysKey { get; set; }

        [Column(Name = "L1")]
        public string L1 { get; set; }

        [Column(Name = "GroupManager")]
        public string GroupManager { get; set; }

        [Column(Name = "Remarks")]
        public string Remarks { get; set; }

        [Column(Name = "History")]
        public string History { get; set; }

        [Column(Name = "Rank")]
        public int Rank { get; set; }

        [Column(Name = "HighestEducation")]
        public string HighestEducation { get; set; }

        [Column(Name = "IndustryExamPassed")]
        public string IndustryExamPassed { get; set; }

        [Column(Name = "IndustryExamPassedDate", IsDate = true)]
        public int? IndustryExamPassedDate { get; set; }

        [Column(Name = "ResetCommissionEffectiveStartDate", IsDate = true)]
        [Display(Name = "Commission Reset Effective Start Date")]
        public int? ResetCommissionEffectiveStartDate { get; set; }
    }
}
