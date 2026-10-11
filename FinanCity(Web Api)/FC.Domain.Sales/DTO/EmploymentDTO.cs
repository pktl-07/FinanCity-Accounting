namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class EmploymentDTO : QueryAssert<EmploymentDTO>
    {
        #region Ctor

        public EmploymentDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.Franchisee).Custom("tbl");
            RuleFor(x => x.R1).Custom("tbl");
            RuleFor(x => x.Name).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.CEANumber).Custom("tbl");
            RuleFor(x => x.Department).Custom("tbl");
            RuleFor(x => x.Designation).Custom("tbl");
            RuleFor(x => x.CommissionScheme).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.Status).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
            RuleFor(x => x.VendorSK).Custom("tbl");
            RuleFor(x => x.ManagerName).Custom("tbl");
            RuleFor(x => x.DirectorName).Custom("tbl");
            RuleFor(x => x.MobilePhone).Custom("tbl");
        }

        #endregion

        #region Properties

        public string SysKey { get; set; }
        
        public int TransType { get; set; }
        
        public string R1 { get; set; }
        
        public string Name { get; set; }
        
        public string NRIC { get; set; }
        
        public string Address { get; set; }
        
        public string CEANumber { get; set; }
        
        public string Department { get; set; }
        
        public string Designation { get; set; }
        
        public string CommissionScheme { get; set; }
        
        public bool IsActive { get; set; }
        
        public string Status { get; set; }
        
        public string Franchisee { get; set; }
        
        public string ManagerName { get; set; }

        public string DirectorName { get; set; }

        public string VendorSK { get;set;}

        public string MobilePhone { get; set; }

        #endregion
    }
}
