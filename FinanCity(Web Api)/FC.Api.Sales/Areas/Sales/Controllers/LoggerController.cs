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

    [RoutePrefix("Logger")]
    public class LoggerController : BaseApiController
    {
        #region Fields

        Logger _logger;
        KendoGridFilterTransalator _kendoGridFilter;
      
        #endregion

        #region Ctor

        public LoggerController(Logger logger,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _logger = logger;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<LoggerEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LoggerEntity>(h);

                return _logger.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public LoggerEntity Find(string key)
        {
            try
            {
                return _logger.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
