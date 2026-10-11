namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "JUN014")]
    public class FollowUpEntity : EntityBase
    {
        public FollowUpEntity() : base("FC.Biz") { }

        [Column]
        public bool Notify { get; set; }

        [Column]
        public int? StartDate { get; set; }

        [Column]
        public string StartTime { get; set; }

        [Column]
        public int? NotifyMeBeforeStartDate { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string RefSK { get; set; }
    }
}
