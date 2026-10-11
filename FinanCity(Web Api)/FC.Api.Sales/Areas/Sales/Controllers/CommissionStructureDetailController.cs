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

    [RoutePrefix("CommissionSchemeDetail")]
    public class CommissionStructureDetailController : BaseApiController
    {
        #region Fields

        private CommissionStructureDetail _business;
        private KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public CommissionStructureDetailController(CommissionStructureDetail business,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<CommissionStructureDetailEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionStructureDetailEntity>(h);

                return _business.Finds(filter: e).OrderBy(x => x.MinCom);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CommissionStructureDetailEntity Find(string key)
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

        #endregion
    }
}
