namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("PropertyTransactionDetail")]
    public class PropertyTransactionDetailController : BaseApiController
    {
        #region Fields

        private PropertyTransactionDetail _propertyTransactionDetail;
        private KendoGridFilterTransalator _kendoGridFilter;
        private PropertyTransaction _propertyTransaction;
     
        #endregion

        #region Ctor

        public PropertyTransactionDetailController(
            PropertyTransaction propertyTransaction,
            PropertyTransactionDetail propertyTransactionDetail,
            KendoGridFilterTransalator kendoGridFilter,
            IMultiCurrencyConverter currencyConverter)
        {
            _propertyTransaction = propertyTransaction;
            _propertyTransactionDetail = propertyTransactionDetail;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<PropertyTransactionDetailEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PropertyTransactionDetailEntity>(h);

                e.CreateObject((mem, value) =>
                {
                    if (mem == $@"{nameof(PropertyTransactionDetailEntity.PTOSK)}")
                    {
                        if (value != null)
                        {
                            var key = _propertyTransaction.GetSingleKey(value);
                            e = e.Constant(c => c.PTOSK == key);
                        }
                    }
                });
                return _propertyTransactionDetail.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PropertyTransactionDetailEntity Find(string key)
        {
            try
            {
                return _propertyTransactionDetail.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
