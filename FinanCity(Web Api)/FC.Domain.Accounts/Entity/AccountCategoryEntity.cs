namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;

    [Table(Name = "ACC004")]
    [Description("Account Category")]
    public class AccountCategoryEntity : EntityBase
    {
        public AccountCategoryEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "CategoryDescription"),DisplayName("Category Description")]
        public string CategoryDescription
        {
            get;
            set;
        }
    }
}
