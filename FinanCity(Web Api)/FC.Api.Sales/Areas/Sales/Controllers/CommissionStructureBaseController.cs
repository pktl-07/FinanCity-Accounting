namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("CommissionStructureBase")]
    public class CommissionStructureBaseController : BaseApiController
    {
        #region Fields

        private CommissionStructureBase _commission;
        private KendoGridFilterTransalator _kendoGridFilter;
       
        #endregion

        #region Ctor

        public CommissionStructureBaseController(CommissionStructureBase commissionScheme,
            CommissionSchemeValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _commission = commissionScheme;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<CommissionStructureBaseEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionStructureBaseEntity>(h);

                return _commission.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CommissionStructureBaseEntity Find(string key)
        {
            try
            {
                return _commission.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
