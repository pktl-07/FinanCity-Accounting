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

    [RoutePrefix("MarketingExpense")]
    public class MarketingExpenseController : BaseApiController
    {
        #region Fields

        MarketingExpense _promotionItemExpense;
        KendoGridFilterTransalator _kendoGridFilter;
   
        #endregion

        #region Ctor

        public MarketingExpenseController(MarketingExpense promotionItemExpense,
            QualificationValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _promotionItemExpense = promotionItemExpense;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<MarketingExpenseEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<MarketingExpenseEntity>(h);

                return _promotionItemExpense.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public MarketingExpenseEntity Find(string key)
        {
            try
            {
                return _promotionItemExpense.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
