namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
  
    [Table(Name = "ACC006")]
    public class AccountingPeriodEntity : EntityBase
    {
        public AccountingPeriodEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "YC")]
        public string YC
        {
            get;
            set;
        }

        [Column(Name = "PC")]
        public string PC
        {
            get;
            set;
        }

        [Column(Name = "Date1")]
        public int Date1
        {
            get;
            set;
        }

        [Column(Name = "Date2")]
        public int Date2
        {
            get;
            set;
        }

        [Column(Name = "BatchEntryStatus")]
        public int BatchEntryStatus
        {
            get;
            set;
        }

        [Column(Name = "IsDefaultYear")]
        public bool IsDefaultYear
        {
            get;
            set;
        }
    }
}
