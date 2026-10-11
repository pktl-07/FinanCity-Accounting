namespace FC.Core.Infrastructure.Interfaces
{
    using System;
    using System.Web;
    using System.Security.Claims;
    using System.Linq;
    using FC.Core;

    public interface IFinanCityIdentity
    {
        string UID { get; }
        int AccountType { get; }
        string SPSK { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime LoggedOn { get; }
        UserAccountType UserAccountType { get; } 
    }

    public class FinanCityIdentity : IFinanCityIdentity
    {
        public FinanCityIdentity()
        {
            try
            {
                var identityClaims = (ClaimsIdentity)HttpContext.Current?.User?.Identity;
                if (identityClaims != null)
                {
                    var claim = identityClaims.Claims.FirstOrDefault();
                    if (claim != null)
                    {
                        this._uid = claim.Subject.FindFirst("UID").Value;
                        this._accountype = int.Parse(claim.Subject.FindFirst("AccountType").Value);
                        this._spsk = claim.Subject.FindFirst("SPSK").Value;
                        this._email = claim.Subject.FindFirst("Email").Value;
                        this._firstName = claim.Subject.FindFirst("FirstName").Value;
                        this._lastName = claim.Subject.FindFirst("LastName").Value;
                        this._loggedon = DateTime.Parse(claim.Subject.FindFirst("LoggedOn").Value);

                        switch (_accountype)
                        {
                            case 1:
                                _userAccountType = UserAccountType.AdminAccount;
                                break;
                            case 2:
                                _userAccountType = UserAccountType.SalespersonAccount;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string _uid;
        public string UID
        {
            get
            {
                return _uid;
            }
        }

        private int _accountype;
        public int AccountType
        {
            get
            {
                return _accountype;
            }
        }

        private string _spsk;
        public string SPSK
        {
            get
            {
                return _spsk;
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
        }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
        }

        private DateTime _loggedon;
        public DateTime LoggedOn
        {
            get
            {
                return _loggedon;
            }
        }

        private UserAccountType _userAccountType;
        public UserAccountType UserAccountType
        {
            get
            {
                return _userAccountType;
            }
        }
    }
}
