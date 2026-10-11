namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class LeadFollowUpDTO : QueryAssert<LeadFollowUpDTO>
    {
        #region Ctor

        public LeadFollowUpDTO() : base("FC.Biz")
        {
            RuleFor(x => x.FollowUpSK).Custom("tbl");
            RuleFor(x => x.Description).Custom("tbl");
            RuleFor(x => x.LeadSK).Custom("tbl");
            RuleFor(x => x.SenderSK).Custom("tbl");
            RuleFor(x => x.ReceiverSK).Custom("tbl");
            RuleFor(x => x.LeadName).Custom("tbl");
            RuleFor(x => x.StartDate).Custom("tbl");
            RuleFor(x => x.StartTime).Custom("tbl");
            RuleFor(x => x.NotificationSK).Custom("tbl");
            RuleFor(x => x.IsRead).Custom("tbl");
        }

        #endregion

        #region Properties
        
        public bool IsRead { get; set; }

        public string NotificationSK { get; set; }

        public string FollowUpSK { get; set; }
       
        public int? StartDate { get; set; }

        public string StartTime { get; set; }

        public string LeadName { get; set; }

        public string Description { get; set; }
        
        public string LeadSK { get; set; }
        
        public string SenderSK { get; set; }
        
        public string ReceiverSK { get; set; }
     
        #endregion
    }
}
