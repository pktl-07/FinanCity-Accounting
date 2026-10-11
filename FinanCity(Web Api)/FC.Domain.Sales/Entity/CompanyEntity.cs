namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Company")]
    [RecordIndexer(AutoGenerateIdReferencesOnSave = true)]
    public partial class CompanyEntity : EntityBase , IReferencesConstraint
        , IUniqueFieldConstraint<CompanyEntity>   
    {
        public CompanyEntity() : base("FC.Biz") { }

        [Column(Name = "CompanyName"), Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Column(Name = "CompanyReg"), Display(Name = "Company Registration Number")]
        public string CompanyReg { get; set; }

        [Column(Name = "AgentLicense")]
        public string AgentLicense { get; set; }

        [Column(Name = "GSTRegistration")]
        public string GSTRegistration { get; set; }

        [Column(Name = "GSTEffective")]
        public string GSTEffective { get; set; }

        [Column(Name = "GSTPercent"), DataType(DataType.Currency, ErrorMessage = "GST percent must be number")]
        public decimal GSTPercent { get; set; }

        [Column(Name = "TaxSK"),Display(Name = "GST")]
        public string TaxSK { get; set; }

        [Column(Name = "TransType"),Display(Name = "Vendor Type")]
        public int TransType { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "ContactPerson")]
        public string ContactPerson { get; set; }

        [Column(Name = "OpenDate"), Display(Name = "Open Date")]
        public int? OpenDate { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        [Column(Name = "RefSP")]
        public string RefSP { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PersonEntity>(x => x.VendorSK);
        }

        public void UniqueField(IUniqueFieldAnalyzer<CompanyEntity> action)
        {
            action.Register(x => new object[] { x.CompanyReg });
        }
    }
}
