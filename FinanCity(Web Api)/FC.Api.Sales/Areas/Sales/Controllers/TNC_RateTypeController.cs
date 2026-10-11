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

    [RoutePrefix("TNC_RateType")]
    public class TNC_RateTypeController : BaseApiController
    {
        #region Fields

        TNC_RateType _rateType;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public TNC_RateTypeController(TNC_RateType rateType,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _rateType = rateType;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<TNC_RateTypeEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<TNC_RateTypeEntity>(h);

                return _rateType.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public TNC_RateTypeEntity Find(string key)
        {
            try
            {
                return _rateType.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public TNC_RateTypeEntity Default()
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
        public IHttpActionResult Save(TNC_RateTypeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _rateType.Insert(model);

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
        public IHttpActionResult Update(TNC_RateTypeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _rateType.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_rateType.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _rateType.Delete(filter: f => f.SysKey == syskey);

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

        private TNC_RateTypeEntity GetDefaultModel()
        {
            return new TNC_RateTypeEntity { IsActive = true };
        }

        #endregion
    }
}
