namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class SalesPersonTireDTO : QueryAssert<LeadAndContactDTO>
    {
        #region Ctor

        public SalesPersonTireDTO() : base("FC.Biz")
        {
        }

        #endregion

        #region Properties

        public int RowNumber { get; set; }

        public string SPSK { get; set; }
        
        public string Name { get; set; }
        
        public string ParentSK { get; set; }
        
        public string Designation { get; set; }
        
        public int Level { get; set; }
        
        public int Rank { get; set; }
        
        public int GroupNum { get; set; }
        
        public bool IsActive { get; set; }

        public string NRIC { get; set; }

        public decimal Payout { get; set; }

        #endregion
    }
}
