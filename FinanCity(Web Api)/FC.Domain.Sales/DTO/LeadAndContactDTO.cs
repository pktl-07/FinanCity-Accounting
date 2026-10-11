namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class LeadAndContactDTO : QueryAssert<LeadAndContactDTO>
    {
        #region Ctor

        public LeadAndContactDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.R1).Custom("tbl");
            RuleFor(x => x.Name).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.Mobile).Custom("tbl");
            RuleFor(x => x.Email).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
            RuleFor(x => x.Fax).Custom("tbl");
            RuleFor(x => x.CompanyName).Custom("tbl");
            RuleFor(x => x.IsOrganization).Custom("tbl");
            RuleFor(x => x.VendorType).Custom("tbl");
            RuleFor(x => x.RefSK).Custom("tbl");
            RuleFor(x => x.VendorSK).Custom("tbl");
            RuleFor(x => x.Remark).Custom("tbl");
            RuleFor(x => x.Designation).Custom("tbl");
        }

        #endregion

        #region Methods
        public string SysKey { get; set; }
        public string R1 { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string NRIC { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public bool IsActive { get; set; }
        public int TransType { get; set; }
        public string CompanyName { get; set; }
        public bool IsOrganization { get; set; }
        public int VendorType { get; set; }
        public string RefSK { get; set; }
        public string VendorSK { get; set; }
        public string Designation { get; set; }
        public string Remark { get; set; }

        #endregion
    }

    public class LeadAndSalespersonDTO : QueryAssert<LeadAndSalespersonDTO>
    {
        #region Ctor

        public LeadAndSalespersonDTO() : base("FC.Biz")
        {
            RuleFor(x => x.LeadSK).Custom("tbl");
            RuleFor(x => x.R1).Custom("tbl");
            RuleFor(x => x.Name).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.RefSK).Custom("tbl");
        }

        #endregion

        #region Methods

        public string LeadSK { get; set; }
        public string R1 { get; set; }
        public string Name { get; set; }
        public string NRIC { get; set; }
        public bool IsActive { get; set; }
        public string RefSK { get; set; }

        #endregion
    }
}
