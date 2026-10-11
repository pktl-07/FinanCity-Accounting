namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
  
    [Table(Name = "ACC001")]
    [Description("Account")]
    public class AccountSetupEntity : EntityBase 
    {
        public AccountSetupEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "AccCode")]
        public string AccCode { get; set; }
      
        [Column(Name = "AccDescription"), Display(Name = "Account Name")]
        public string AccDescription { get; set; }
        
        [Column(Name = "CategorySK"), Display(Name = "Account Category")]
        public string CategorySK { get; set; }
      
        [Column(Name = "AccountType")]
        public int AccountType { get; set; }
        
        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }
       
        [Column(Name = "DRCR")]
        public string DRCR { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<AccountSetupEntity> action)
        {
            action.Register(f => new object[] { f.AccCode });
            action.Register(f => new object[] { f.AccDescription });
        }
    }
}
