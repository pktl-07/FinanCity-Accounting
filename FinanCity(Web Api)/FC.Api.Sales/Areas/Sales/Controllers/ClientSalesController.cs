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

    [RoutePrefix("ClientSales")]
    public class ClientSalesController : BaseApiController
    {
        #region Fields

        PTOClient _ptoClient;
        private KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public ClientSalesController(PTOClient ptoClient,
             KendoGridFilterTransalator kendoGridFilter)
        {
            _ptoClient = ptoClient;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<PTOClientEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PTOClientEntity>(h);
                var p = _ptoClient.Finds(filter: e).ToList();

                return _ptoClient.Finds(filter: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        #endregion
    }
}
