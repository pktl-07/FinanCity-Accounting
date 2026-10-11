namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;

    [RoutePrefix("AccountCategory")]
    public class AccountCategoryController : BaseApiController
    {
        #region Fields

        private readonly AccountCategory _business;
        private readonly KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public AccountCategoryController(AccountCategory business,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<AccountCategoryEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AccountCategoryEntity>(h);

                return _business.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public AccountCategoryEntity Find(string key)
        {
            try
            {
                return _business.Finds(filter:f=>f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
