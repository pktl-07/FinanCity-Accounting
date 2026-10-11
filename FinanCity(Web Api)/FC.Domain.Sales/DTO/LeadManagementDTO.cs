namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class LeadManagementDTO : QueryAssert<LeadManagementDTO>
    {
        #region Ctor

        public LeadManagementDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.LeadNo).Custom("tbl");
            RuleFor(x => x.Name).Custom("tbl");
            RuleFor(x => x.IsOrganization).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.PropertyType).Custom("tbl");
            RuleFor(x => x.Mobile).Custom("tbl");
            RuleFor(x => x.Email).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.RefSK).Custom("tbl");
            RuleFor(x => x.SalesActivity).Custom("tbl");
            RuleFor(x => x.FollowupDate).Custom("tbl");
            RuleFor(x => x.LeadCloseRate).Custom("tbl");
            RuleFor(x => x.SalespersonName).Custom("tbl");
        }

        #endregion

        #region Properties
        
        public int LeadCloseRate { get; set; }
        public int? FollowupDate { get; set; }
        public string SysKey { get; set; }
        public string LeadNo { get; set; }
        public string SalesActivity { get; set; }
        public string Name { get; set; }
        public bool IsOrganization { get; set; }
        public string NRIC { get; set; }
        public string Address { get; set; }
        public string PropertyType { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int TransType { get; set; }
        public string RefSK { get; set; }

        public string SalespersonName { get; set; }

        #endregion
    }
}
