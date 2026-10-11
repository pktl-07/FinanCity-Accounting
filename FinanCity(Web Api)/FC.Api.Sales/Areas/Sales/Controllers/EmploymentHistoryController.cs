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

    [RoutePrefix("EmploymentHistory")]
    public class EmploymentHistoryController : BaseApiController
    {
        #region Fields

        private EmploymentHistory _business;
        private KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public EmploymentHistoryController(EmploymentHistory business,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<EmploymentHistoryEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<EmploymentHistoryEntity>(h);

                return _business.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public EmploymentHistoryEntity Find(string key)
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
