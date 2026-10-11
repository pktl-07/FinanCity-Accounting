namespace FC.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Security.Claims;
    using System.Web;
    using System.Net;
    using System.Web.Http;
    using Ninject;
    using FluentValidation;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Api.Infrastructure.FluentValidators;
    using FC.Api.Models;
    using FC.Domain.Auth.Business;
    using FC.Domain.Auth.Entity;

    [RoutePrefix("UserAccount")]
    public class UserAccountController : BaseApiController
    {
        #region Fields

        private readonly UserAccount _userAccount;
        private readonly KendoGridFilterTransalator _kendoGridFilter;
        private readonly UserAccountValidator _userAccountValidator;
        private readonly DeleteUserAccountValidator _deleteUserAccountValidator;
        private readonly ChangePasswordValidator _changePasswordValidator;
        private readonly ResetPasswordValidator _resetPasswordValidator;
        
        public IEssAppRepository App 
        {
            get => DI.Current.Get<IEssAppRepository>();
        }

        #endregion

        #region Ctor

        public UserAccountController(UserAccount userAccount,
             KendoGridFilterTransalator kendoGridFilter,
             UserAccountValidator userAccountValidator,
             DeleteUserAccountValidator deleteUserAccountValidator,
             ResetPasswordValidator resetPasswordValidator,
             ChangePasswordValidator changePasswordValidator)
        {
            _resetPasswordValidator = resetPasswordValidator;
            _userAccount = userAccount;
            _kendoGridFilter = kendoGridFilter;
            _userAccountValidator = userAccountValidator;
            _deleteUserAccountValidator = deleteUserAccountValidator;
            _changePasswordValidator = changePasswordValidator;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("GetUserClaims")]
        public UserAccountModel GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            var model = new UserAccountModel
            {
                UserAccountName = identityClaims.FindFirst(ClaimTypes.Name).Value,
                CID = identityClaims.FindFirst("UID").Value,
                SPSK = identityClaims.FindFirst("SPSK").Value,
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = identityClaims.FindFirst("FirstName").Value,
                LastName = identityClaims.FindFirst("LastName").Value,
                LoggedOn = identityClaims.FindFirst("LoggedOn").Value,
                Api = identityClaims.FindFirst("Api").Value
            };
            return model;
        }

        [HttpPut]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                var identityClaims = (ClaimsIdentity)User.Identity;
                var key = identityClaims.FindFirst("UID").Value;
                var user = _userAccount.Finds(filter: f => f.SysKey == key).Single();

                model.UID = key;
                model.Password = user.Password;
                _changePasswordValidator.ValidateAndThrow(model);

                user.Password = model.NewPassword;
                _userAccount.Update(user, filter: f => f.SysKey == key);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public IHttpActionResult ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var key = model.SysKey;
                var user = _userAccount.Finds(filter: f => f.SysKey == key).Single();

                _resetPasswordValidator.ValidateAndThrow(model);

                user.Password = model.NewPassword;
                _userAccount.Update(user, filter: f => f.SysKey == key);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("RequestResetPassword")]
        [AllowAnonymous]
        public IHttpActionResult RequestResetPassword(RequestResetPasswordModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var user = _userAccount.Finds(filter: f => f.Google == model.Email).SingleOrDefault();

                    if (user == null)
                        throw new Exception("Invalid user.");

                    var message = new MailMessage();
                    var body = this.WriteResetPasswordTemplate(user.SysKey);

                    message.To.Add(new MailAddress(model.Email));
                    message.Subject = "Password reset";
                    message.Body = string.Format(body, "", "", "");
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                    }

                    scope.Complete();

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<UserAccountEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<UserAccountEntity>(h);
                var api = DI.Current.Get<IEssAppRepository>().DomainUrl;

                e = e.Constant(a => a.Api == api);

                return _userAccount.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<UserAccountEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<UserAccountEntity>(h);
                var api = DI.Current.Get<IEssAppRepository>().DomainUrl;

                e = e.Constant(a => a.Api == api);
                return _userAccount.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public UserAccountEntity Find(string key)
        {
            try
            {
                return _userAccount.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public UserAccountEntity Default()
        {
            try
            {
                var api = DI.Current.Get<IEssAppRepository>().DomainUrl;
                return new UserAccountEntity
                {
                    IsActive = true,
                    IsDefault = false,
                    AccountType = (int)UserAccountType.SalespersonAccount,
                    Api = api,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(UserAccountEntity model)
        {
            try
            {
                _userAccountValidator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Password = RandomString(6);
                    model.SPSK = model.SPSK == null ? "1" : model.SPSK;

                    model = _userAccount.Insert(model);

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("ess.earthstone@gmail.com");
                        mail.To.Add(model.Google);
                        mail.Subject = "FinanCity";
                        mail.Body = WriteCreatePasswordTemplate(model);
                        mail.IsBodyHtml = true;
                     
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential("ess.earthstone@gmail.com", 
                                "ess@123123");
                            smtp.Send(mail);
                        }
                    }
                  
                    scope.Complete();
                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(UserAccountEntity model)
        {
            try
            {
                _userAccountValidator.ValidateAndThrow(model);
                _userAccount.Update(model, filter: f => f.SysKey == model.SysKey);

                return Ok(_userAccount.Finds(filter: f => f.SysKey == model.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(string syskey)
        {
            try
            {
                _deleteUserAccountValidator.ValidateAndThrow(_userAccount.Finds(filter: f => f.SysKey == syskey).SingleOrDefault());

                _userAccount.Delete(filter: f => f.SysKey == syskey);

                return Ok(new UserAccountEntity { IsActive = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utilities

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string WriteCreatePasswordTemplate(UserAccountEntity model)
        {
            var html = $@"
            <html>
               <span>Dear User</span>
               <br>
               You have subscribed to {App.FrontEndUrl} by our admin.You can login with the following user accounts.
               <br>
               <h4 style='text-decoration: underline;'>Password account</h4>
               <ul style='list-style-type: none;'>
                    <li>user name: {model.PasswordLoginUserName}</li>
                    <li> password: {model.Password}</li>
               </ul>
               <h4 style='text-decoration: underline;'>G-Mail account</h4>
               <ul style='list-style-type: none;'>
                    <li>user name: {model.Google}</li>
               </ul>
               <h4 style='text-decoration: underline;'>Facebook account</h4>
               <ul style='list-style-type: none;'>
                    <li>user name: {model.Facebook ?? "N/A"}</li>
               </ul>
               <br>
                    <span>Best Regards,</span>
               <br>
               Eearth Stone Solution's Admin.
            </html>";
            return html;
        }

        private string WriteResetPasswordTemplate(string syskey)
        {
            var html = $@"
            <html>
               <span>Dear User</span>
               <br>
               You reset your password. To proceed this kindly press the following URL link.
               <br>
               {App.FrontEndUrl}/Users/ResetPassword/{syskey}
               <span>Best Regards,</span>
               <br>
               Eearth Stone Solution's Admin.
            </html>";
            return html;
        }

        #endregion
    }
}
