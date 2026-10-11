namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
 
    [RoutePrefix("PostalCode")]
    public class PostalCodeController : BaseApiController
    {
        #region Fields

        PostalCode _postalCode;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public PostalCodeController(PostalCode postalCode,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _postalCode = postalCode;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("getByKey/{key}")]
        public FullAddressDTO Find(string key)
        {
            try
            {
                return _postalCode.FindFullAddress(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
