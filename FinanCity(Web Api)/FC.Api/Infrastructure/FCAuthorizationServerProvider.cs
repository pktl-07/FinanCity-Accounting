namespace FC.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FC.Core;
    using Ninject;
    using Microsoft.Owin.Security.OAuth;
    using FC.Domain.Auth.Business;
    using FC.Domain.Auth.Entity;

    public delegate void Authenticate(UserAccountEntity user);
    
    public class FCAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        #region Events

        public static event Authenticate OnAuthenticate;

        #endregion

        #region Fields

        private UserAccount _userAccount;

        #endregion

        #region Ctor

        public FCAuthorizationServerProvider(UserAccount userAccount)
        {
            _userAccount = userAccount;
        }

        #endregion

        #region Methods

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                var loginMode = context.Parameters.Where(f => f.Key == "login_mode").Select(f => f.Value).SingleOrDefault()[0];
                var password = context.Parameters.Where(f => f.Key == "password").Select(f => f.Value).SingleOrDefault()[0];
                var username = context.Parameters.Where(f => f.Key == "username").Select(f => f.Value).SingleOrDefault()[0];
                var external = context.Parameters.Where(f => f.Key == "external").Select(f => f.Value).SingleOrDefault();
                var social = external != null ? external[0].ToLower() : null;
               
                var user = GetUser(userName: username,
                    password: password,
                    external: social);

                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new Exception($@"User name or email is empty.");
                }
                else if (user == null)
                {
                    if (GetUserByNameAndType(username: username,
                        external: social) != null)
                        throw new Exception(@"Incorrect password.");

                    throw new Exception($@"{username} is not a registered user.");
                }
                else if (loginMode.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (user.AccountType != (int)UserAccountType.AdminAccount
                        || user.SPSK != "1")

                        throw new Exception("Invalid login for admin user.");
                }
                else if (loginMode == "salesperson")
                {
                    if (user.AccountType != (int)UserAccountType.SalespersonAccount
                        || string.IsNullOrWhiteSpace(user.SPSK))

                        throw new Exception("Invalid login for salesperson user.");
                }
                else
                {
                    throw new Exception("Invalid login account.");
                }

                OnAuthenticate?.Invoke(user);
                context.Validated();
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", ex.Message);
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var username = context.UserName;
                var password = context.Password;
                var api = DI.Current.Get<IEssAppRepository>().DomainUrl;
                var user = _userAccount.Finds(filter: (f) =>
                (f.PasswordLoginUserName == username 
                || f.Google == username
                || f.Apple == username
                || f.Facebook == username)
                  && (f.Password == password
                  || f.GoogleId == password
                  || f.AppleId == password
                  || f.FacebookId == password)
                  && f.Api == api
                  && f.IsActive == true).SingleOrDefault();

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.PasswordLoginUserName));
                identity.AddClaim(new Claim("UID", user.SysKey));
                identity.AddClaim(new Claim("AccountType", user.AccountType.ToString()));
                identity.AddClaim(new Claim("SPSK", user.SPSK ?? "1"));
                identity.AddClaim(new Claim("Email", user.Google));
                identity.AddClaim(new Claim("FirstName", user.FirstName));
                identity.AddClaim(new Claim("LastName", user.LastName));
                identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                identity.AddClaim(new Claim("Api", user.Api));

                context.Validated(identity);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", ex.Message);
            }
        }

        #endregion

        #region Utilities

        private UserAccountEntity GetUser(
            string userName,
            string password,
            string external)
        {
            UserAccountEntity user = null;
            var api = DI.Current.Get<IEssAppRepository>().DomainUrl;

            switch (external)
            {
                case "google":
                    
                    user = _userAccount.Finds(filter: (f) =>
                    f.Google == userName
                    && f.IsActive == true
                    && f.Api == api).SingleOrDefault();

                    if (user != null)
                    {
                        _userAccount.Update((s) => new UserAccountEntity
                        {
                            GoogleId = password
                        }, (f) => f.Google == userName);
                    }
                    break;

                case "facebook":

                    user = _userAccount.Finds(filter: (f) =>
                                f.Facebook == userName
                                && f.IsActive == true
                                && f.Api == api)
                            .FirstOrDefault();

                    if (user != null)
                    {
                        _userAccount.Update((s) => new UserAccountEntity
                        {
                            FacebookId = password
                        }, (f) => f.Facebook == userName);
                    }
                    break;

                case "ios":

                    user = _userAccount.Finds(filter: (f) =>
                                f.Apple == userName
                                && f.IsActive == true
                                && f.Api == api)
                            .FirstOrDefault();

                    if (user != null)
                    {
                        _userAccount.Update((s) => new UserAccountEntity
                        {
                            AppleId = password
                        }, (f) => f.Apple == userName);
                    }
                    break;

                default:
                    //generic user name and passwrod account.
                    user = _userAccount.Finds(filter: (f) =>
                            (f.PasswordLoginUserName == userName
                            || f.Facebook == userName
                            || f.Apple == userName
                            || f.Google == userName)
                               && f.Password == password
                               && f.Api == api
                               && f.IsActive == true).FirstOrDefault();
                    break;
            }

            return user;
        }

        private UserAccountEntity GetUserByNameAndType(string username,
          string external)
        {
            switch (external)
            {
                case "google": return _userAccount.Finds(filter: f => f.Google == username).SingleOrDefault();
                case "facebook": return _userAccount.Finds(filter: f => f.Facebook == username).SingleOrDefault();
                case "ios": return _userAccount.Finds(filter: f => f.Apple == username).SingleOrDefault();
                default:
                    return _userAccount.Finds(filter: f => f.PasswordLoginUserName == username).SingleOrDefault();
            }
        }

        #endregion
    }
}