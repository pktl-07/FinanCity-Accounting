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

    [RoutePrefix("PayNow")]
    public class PayNowController : BaseApiController
    {
        #region Fields

        PayNow _paynow;
        KendoGridFilterTransalator _kendoGridFilter;
     
        #endregion

        #region Ctor

        public PayNowController(PayNow paynow,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _paynow = paynow;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<PayNowEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PayNowEntity>(h);

                return _paynow.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PayNowEntity Find(string key)
        {
            try
            {
                return _paynow.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
