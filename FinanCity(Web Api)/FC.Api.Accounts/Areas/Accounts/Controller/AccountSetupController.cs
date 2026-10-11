namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FluentValidation;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;

    [RoutePrefix("AccountSetup")]
    public class AccountSetupController : BaseApiController
    {
        #region Fields

        AccountSetup _business;
        KendoGridFilterTransalator _kendoGridFilter;
        AccountSetupValidator _validator;

        #endregion

        #region Ctor

        public AccountSetupController(AccountSetup business,
            AccountSetupValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _business = business;
            _validator = validator;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<AccountSetupEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AccountSetupEntity>(h);

                return _business.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public AccountSetupEntity Find(string key)
        {
            try
            {
                return _business.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public AccountSetupEntity Default()
        {
            try
            {
                return GetDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(AccountSetupEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                return Ok(_business.Insert(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(AccountSetupEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                _business.Update(model, filter: f => f.SysKey == model.SysKey);

                return Ok(_business.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                _business.Delete(filter: f => f.SysKey == syskey);

                return Ok(GetDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private AccountSetupEntity GetDefault()
        {
            return new AccountSetupEntity
            {
                AccountType = (int)FC.Core.AccountType.Posting,
                IsActive = true
            };
        }

        #endregion
    }
}
