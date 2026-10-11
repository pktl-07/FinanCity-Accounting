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

    [RoutePrefix("PaymentTerms")]
    public class PaymentTermController : BaseApiController
    {
        #region Fields

        PaymentTerm _paymentTerm;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public PaymentTermController(PaymentTerm paymentTerm,
          QualificationValidator validator,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _paymentTerm = paymentTerm;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<PaymentTermEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PaymentTermEntity>(h);

                return _paymentTerm.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PaymentTermEntity Find(string key)
        {
            try
            {
                return _paymentTerm.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
