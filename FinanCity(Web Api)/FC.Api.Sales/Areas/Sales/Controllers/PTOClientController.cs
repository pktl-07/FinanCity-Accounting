namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("PTOClient")]
    public class PTOClientController : BaseApiController
    {
        #region Fields

        PTOClient _ptoClient;
        KendoGridFilterTransalator _kendoGridFilter;
        SalesInvoice _salesInvoice;
      
        #endregion

        #region Ctor

        public PTOClientController(PTOClient ptoClient,
            SalesInvoice salesInvoice,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _salesInvoice = salesInvoice;
            _ptoClient = ptoClient;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<PTOClientEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PTOClientEntity>(h);

                e.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(PTOClientEntity.PTOSK)}")
                    {
                        if (value != null)
                        {
                            var key = _salesInvoice.GetSingleKey(value);

                            e = e.Constant(c => 1 == 1).Erase($"{nameof(PTOClientEntity.PTOSK)}");
                            e = e.Constant(c => c.PTOSK == key);
                        }
                    }
                });

                return _ptoClient.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<PTOClientEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PTOClientEntity>(h);

                return _ptoClient.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PTOClientEntity Find(string key)
        {
            try
            {
                return _ptoClient.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
