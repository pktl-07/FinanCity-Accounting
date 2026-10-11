namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    
    [Table(Name = "EMP_History")]
    public class EmploymentHistoryEntity : EntityBase
    {
        public EmploymentHistoryEntity() : base("FC.Biz") { }

        [Column(Name = "PreviousAgency")]
        public string PreviousAgency { get; set; }

        [Column(Name = "PreviousAgencyDesignation")]
        public string PreviousAgencyDesignation { get; set; }

        [Column(Name = "YearsOfExperience")]
        public int YearsOfExperience { get; set; }

        [Column(Name = "CEASubmissionDate")]
        public int? CEASubmissionDate { get; set; }

        [Column(Name = "IPADate", IsDate = true)]
        public int? IPADate { get; set; }

        [Column(Name = "ApprovalDate", IsDate = true)]
        public int? ApprovalDate { get; set; }

        [Column(Name = "SPHOpeningDate", IsDate = true)]
        public int? SPHOpeningDate { get; set; }

        [Column(Name = "PhotoPicOpeningDate", IsDate = true)]
        public int? PhotoPicOpeningDate { get; set; }

        [Column(Name = "JoiningPromotion")]
        public string JoiningPromotion { get; set; }

        [Column(Name = "JoiningFeesAmount")]
        public decimal JoiningFeesAmount { get; set; }

        [Column(Name = "JoiningFeesPaidDate", IsDate = true)]
        public int? JoiningFeesPaidDate { get; set; }

        [Column(Name = "PackageFeesAmount")]
        public decimal PackageFeesAmount { get; set; }

        [Column(Name = "PackageFeesPaidDate", IsDate = true)]
        public int? PackageFeesPaidDate { get; set; }

        [Column(Name = "ProfessionalIndemnityFeesAmount")]
        public decimal ProfessionalIndemnityFeesAmount { get; set; }

        [Column(Name = "ProfessionalIndemnityFeesPaidDate", IsDate = true)]
        public int? ProfessionalIndemnityFeesPaidDate { get; set; }

        [Column(Name = "EmailArchiveFeesAmount")]
        public decimal EmailArchiveFeesAmount { get; set; }

        [Column(Name = "EmailArchiveFeesPaidDate", IsDate = true)]
        public int? EmailArchiveFeesPaidDate { get; set; }

        [Column(Name = "CEARegistrationFeesAmount")]
        public decimal CEARegistrationFeesAmount { get; set; }

        [Column(Name = "CEARegistrationFeesPaidDate", IsDate = true)]
        public int? CEARegistrationFeesPaidDate { get; set; }

        [Column(Name = "DateJoin", IsDate = true)]
        public int? DateJoin { get; set; }

        [Column(Name = "NameCardOrder")]
        public decimal NameCardOrder { get; set; }

        //[Column(Name = "NameCardOrderPaidDate", IsDate = true)]
        //public string NameCardPaidDate { get; set; }

        [Column(Name = "GIRO")]
        public int GIRO { get; set; }

        [Column(Name = "RecruiterSK")]
        public string RecruiterSK { get; set; }

        [Column(Name = "Remarks")]
        public string Remarks { get; set; }

    }
}
