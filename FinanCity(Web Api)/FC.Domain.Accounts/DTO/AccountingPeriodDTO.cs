namespace FC.Domain.Accounts.DTO
{
    using FC.Core;
    using FC.Core.Data;
   
    public class AccountingPeriodDTO : QueryAssert<AccountingPeriodDTO>
    {
        #region Ctor

        public AccountingPeriodDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.PeriodSK).Custom("tbl");
            RuleFor(x => x.YC).Custom("tbl");
            RuleFor(x => x.PC).Custom("tbl");
            RuleFor(x => x.Date1).Custom("tbl");
            RuleFor(x => x.Date2).Custom("tbl");
            RuleFor(x => x.GLC_Status).Custom("tbl");
            RuleFor(x => x.SOP_Status).Custom("tbl");
            RuleFor(x => x.POP_Status).Custom("tbl");
            RuleFor(x => x.STK_Status).Custom("tbl");
            RuleFor(x => x.CBR_Status).Custom("tbl");
            RuleFor(x => x.CBP_Status).Custom("tbl");
            RuleFor(x => x.EXT1_Status).Custom("tbl");
            RuleFor(x => x.EXT2_Status).Custom("tbl");
        }

        #endregion

        #region Properties

        public string PeriodSK { get; set; }

        public string YC { get; set; }

        public string PC { get; set; }

        public int Date1 { get; set; }

        public int Date2 { get; set; }
        
        public int GLC_Status { get; set; }

        public int SOP_Status { get; set; }

        public int POP_Status { get; set; }

        public int STK_Status { get; set; }

        public int CBR_Status { get; set; }

        public int CBP_Status { get; set; }

        public int EXT1_Status { get; set; }

        public int EXT2_Status { get; set; }

        #endregion
    }
}
