namespace FC.Domain.Accounts.DTO
{
    using FC.Core;
    using FC.Core.Data;

    public class JournalSummaryDTO : QueryAssert<JournalSummaryDTO>
    {
        public JournalSummaryDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.R1).Custom("tbl");
            RuleFor(x => x.SecondRef).Custom("tbl");
            RuleFor(x => x.ParentId).Custom("tbl");
            RuleFor(x => x.PostDate).Custom("tbl");
            RuleFor(x => x.TotalCredit).Custom("tbl");
            RuleFor(x => x.TotalDebit).Custom("tbl");
            RuleFor(x => x.Description).Custom("tbl");
            RuleFor(x => x.BatchNo).Custom("tbl");
            RuleFor(x => x.ModuleCode).Custom("tbl");
            RuleFor(x => x.YC).Custom("tbl");
            RuleFor(x => x.IsOpening).Custom("tbl");
            RuleFor(x => x.RecordStatus).Custom("tbl");
            RuleFor(x => x.CURRSK).Custom("tbl");
            RuleFor(x => x.CURRATE).Custom("tbl");
            RuleFor(x => x.CURROP).Custom("tbl");                    
            RuleFor(x => x.CreatedDate).Custom("tbl");
            RuleFor(x => x.CreatedTime).Custom("tbl");
            RuleFor(x => x.LoginUser).Custom("tbl");
        }

        public string R1 { get; set; }
        public string SecondRef { get; set; }
        public string ParentId { get; set; }
        public int PostDate { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public string Description { get; set; }
        public string BatchNo { get; set; }
        public string ModuleCode { get; set; }
        public string YC { get; set; }
        public bool IsOpening { get; set; }
        public int RecordStatus { get; set; }
        public string LoginUser { get; set; }
        public int CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public string CURRSK { get; set; }
        public decimal CURRATE { get; set; }
        public string CURROP { get; set; }
    }
}
