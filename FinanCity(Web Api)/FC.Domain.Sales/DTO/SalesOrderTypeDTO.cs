namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class SalesOrderTypeDTO : QueryAssert<SalesOrderTypeDTO>
    {
        #region Ctor

        public SalesOrderTypeDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.Description).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.AccountCode).Custom("tbl");
            RuleFor(x => x.AccountDescription).Custom("tbl");
        }

        #endregion

        #region Properties

        public string SysKey { get; set; }
        
        public string Description { get; set; }
        
        public bool IsActive { get; set; }

        public string AccountCode { get; set; }

        public string AccountDescription { get; set; }
    
        #endregion
    }
}
