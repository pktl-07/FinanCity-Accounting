namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Person")]
    [RecordIndexer(AutoGenerateIdReferencesOnSave = true)]
    public class PersonEntity : EntityBase ,
        IUniqueFieldConstraint<PersonEntity>,
        IReferencesConstraint
    {
        public PersonEntity() : base("FC.Biz") { }

        [Column(Name = "Salutation")]
        public string Salutation { get; set; }

        [Column(Name = "Name"), Display(Name = "NRIC Name")]
        public string Name { get; set; }

        [Column(Name = "DisplayName"), Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Column(Name = "CEANumber"), Display(Name = "CEA Number")]
        public string CEANumber { get; set; }

        [Column(Name = "Gender")]
        public int Gender { get; set; }

        [Column(Name = "DOB", IsDate = true)]
        public int? DOB { get; set; }

        [Column(Name = "NRIC"), Display(Name = "NRIC/ACRA No.")]
        public string NRIC { get; set; }

        [Column(Name = "SinStatus")]
        public string SinStatus { get; set; }

        [Column(Name = "Nationality")]
        public string Nationality { get; set; }

        [Column(Name = "IsSpecialRES")]
        public bool IsSpecialRES { get; set; }

        [Column(Name = "BankName")]
        public string BankName { get; set; }

        [Column(Name = "BankBranchCode")]
        public string BankBranchCode { get; set; }

        [Column(Name = "BankAccountName")]
        public string BankAccountName { get; set; }

        [Column(Name = "BankAccountNo")]
        public string BankAccountNo { get; set; }

        [Column(Name = "BranchCode")]
        public string BranchCode { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "VendorSK")]
        public string VendorSK { get; set; }

        [Column(Name = "MartialStatus")]
        public int MartialStatus { get; set; }

        [Column(Name = "Occupation")]
        public string Occupation { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "ReasonToTerminate"), Display(Name = "Reason For Terminate")]
        public int ReasonToTerminate { get; set; }

        [Column(Name = "OpenDate", IsDate = true), Display(Name = "Open Date")]
        public int? OpenDate { get; set; }

        [Column(Name = "Race")]
        public string Race { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        [Column(Name = "IsOrganization")]
        public bool IsOrganization { get; set; }

        [Column(Name = "ResignedDate", IsDate = true), Display(Name = "Resigned Date")]
        public int? ResignedDate { get; set; }

        [Column(Name = "DebtorCode")]
        public string DebtorCode { get; set; }

        [Column(Name = "CreditorCode")]
        public string CreditorCode { get; set; }

        [Column(Name = "IsLastNameFirst")]
        public bool IsLastNameFirst { get; set; }

        [Column(Name = "DepartmentSK")]
        public string DepartmentSK { get; set; }

        [Column(Name = "FirstName")]
        public string FirstName { get; set; }

        [Column(Name = "LastName")]
        public string LastName { get; set; }
        
        [Column(Name = "ContactPerson")]
        public string ContactPerson { get; set; }

        [Column(Name = "IncludeTopSalesReport")]
        public bool IncludeTopSalesReport { get; set; }

        [Column(Name = "HasGiro")]
        public bool HasGiro { get; set; }

        [Column(Name = "FranchiseeSK")]
        public string FranchiseeSK { get; set; }

        [Column(Name = "Designation")]
        public string Designation { get; set; }

        [Column(Name = "Designation")]
        public string CreatedBy { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PropertyTransactionDetailEntity>(x => x.SPSK);
            relation.Register<PTOClientEntity>(x => x.ClientSK);
            relation.Register<PropertyTransactionEntity>(x => x.BankerSK);
        }

        public void UniqueField(IUniqueFieldAnalyzer<PersonEntity> action)
        {
            //action.Register(x => new object[] { x.NRIC });
        }
    }
}
