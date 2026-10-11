namespace FC.Domain.Auth.Entity
{
    using System.ComponentModel;
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "UserAccount")]
    public class UserAccountEntity : EntityBase,
        IUniqueFieldConstraint<UserAccountEntity>
    {
        public UserAccountEntity() : base("FC.Auth") { }

        [Column(Name = "PasswordLoginUserName")]
        public string PasswordLoginUserName { get; set; }

        [Column(Name = "Google")]
        public string Google { get; set; }

        [Column(Name = "Facebook")]
        public string Facebook { get; set; }

        [Column(Name = "Apple")]
        public string Apple { get; set; }

        //should ignore for http client request to see it.
        //so that evertime user update the data googleid should be reset to null value.
        [Column(Name = "GoogleId")]
        public string GoogleId { get; set; }

        //should ignore for http client request to see it.
        //so that evertime user update the data facebookid should be reset null value.
        [Column(Name = "FacebookId")]
        public string FacebookId { get; set; }

        [Column(Name = "AppleId")]
        public string AppleId { get; set; }

        [Column(Name = "Password")]
        public string Password { get; set; }

        [Column(Name = "FirstName")]
        public string FirstName { get; set; }

        [Column(Name = "LastName")]
        public string LastName { get; set; }

        [Column(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "AccountType")]
        public int AccountType { get; set; }

        [Column(Name = "SPSK"), DisplayName("Salesperson Name")]
        public string SPSK { get; set; }

        [Column(Name = "Api")]
        public string Api { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<UserAccountEntity> action)
        {
            action.Register(x => new object[] { x.PasswordLoginUserName });
            action.Register(x => new object[] { x.Google });
            action.Register(x => new object[] { x.Facebook });
            action.Register(x => new object[] { x.Apple });
        }
    }
}
