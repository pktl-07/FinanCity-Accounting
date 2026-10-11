namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;
 
    public class CompanyAndContactDTO : QueryAssert<CompanyAndContactDTO>
    {
        #region Ctor

        public CompanyAndContactDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.VendorNo).Custom("tbl");
            RuleFor(x => x.VendorType).Custom("tbl");
            RuleFor(x => x.CompanyName).Custom("tbl");
            RuleFor(x => x.Address).Custom("tbl");
            RuleFor(x => x.ACRANo).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.ContactPerson).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
        }

        #endregion

        #region Properties

        public string SysKey { get; set; }
        public string VendorNo { get; set; }
        public int VendorType { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string ACRANo { get; set; }
        public string ContactPerson { get; set; }
        public bool IsActive { get; set; }

        public int TransType { get; set; }

        #endregion
    }
}
