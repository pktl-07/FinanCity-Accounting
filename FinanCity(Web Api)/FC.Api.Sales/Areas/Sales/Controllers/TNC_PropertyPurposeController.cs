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

    [RoutePrefix("TNC_PropertyPrupose")]
    public class TNC_PropertyPurposeController : BaseApiController
    {
        #region Fields

        TNC_PropertyPurpose _propertyPurpose;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public TNC_PropertyPurposeController(TNC_PropertyPurpose propertyPurpose,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _propertyPurpose = propertyPurpose;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        //[HttpGet]
        //[Route("pager/{currentPage}/{pageSize}")]
        //public IDataPagerResult<TNC_TransactionTypeEntity> GetPager(int currentPage = 0, int pageSize = 0)
        //{
        //    try
        //    {
        //        var h = HttpContext.Current.Request.Headers["expression"];
        //        var e = _kendoGridFilter.Convert<QualificationEntity>(HttpContext.Current.Request.Headers["expression"]);

        //        return _transactionType.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        [Route("get")]
        public IEnumerable<TNC_PropertyPurposeEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<TNC_PropertyPurposeEntity>(h);

                return _propertyPurpose.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public TNC_PropertyPurposeEntity Find(string key)
        {
            try
            {
                return _propertyPurpose.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public TNC_PropertyPurposeEntity Default()
        {
            try
            {
                return GetDefaultModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(TNC_PropertyPurposeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _propertyPurpose.Insert(model);

                    scope.Complete();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(TNC_PropertyPurposeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyPurpose.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_propertyPurpose.Finds(filter: f => f.SysKey == model.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyPurpose.Delete(filter: f => f.SysKey == syskey);

                    scope.Complete();
                }
                return Ok(GetDefaultModel());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private TNC_PropertyPurposeEntity GetDefaultModel()
        {
            return new TNC_PropertyPurposeEntity { IsActive = true };
        }

        #endregion
    }
}
