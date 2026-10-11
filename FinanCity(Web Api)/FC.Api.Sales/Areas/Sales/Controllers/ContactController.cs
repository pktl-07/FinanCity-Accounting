namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("Contact")]
    public class ContactController : BaseApiController
    {
        #region Fields

        private Contact _business;
        private KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public ContactController(Contact business,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<ContactEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<ContactEntity>(h);

                return _business.Finds(filter:e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public ContactEntity Find(string key)
        {
            try
            {

                return _business.Finds(f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
