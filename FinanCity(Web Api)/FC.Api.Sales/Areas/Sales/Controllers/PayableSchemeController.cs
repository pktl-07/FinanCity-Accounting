namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("PayableScheme")]
    public class PayableSchemeController : BaseApiController
    {
        #region Fields

        PayableScheme _payableScheme;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public PayableSchemeController(PayableScheme payableScheme,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _payableScheme = payableScheme;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<PayableSchemeEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PayableSchemeEntity>(h);

                return _payableScheme.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
