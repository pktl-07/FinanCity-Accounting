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

    [RoutePrefix("TNC_TransactionType")]
    public class TNC_TransactionTypeController : BaseApiController
    {
        #region Fields

        TNC_TransactionType _transactionType;
        KendoGridFilterTransalator _kendoGridFilter;
    
        #endregion

        #region Ctor

        public TNC_TransactionTypeController(TNC_TransactionType transactionType,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _transactionType = transactionType;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<TNC_TransactionTypeEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<TNC_TransactionTypeEntity>(h);

                return _transactionType.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public TNC_TransactionTypeEntity Find(string key)
        {
            try
            {
                return _transactionType.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public TNC_TransactionTypeEntity Default()
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
        public IHttpActionResult Save(TNC_TransactionTypeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _transactionType.Insert(model);

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
        public IHttpActionResult Update(TNC_TransactionTypeEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _transactionType.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_transactionType.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _transactionType.Delete(filter: f => f.SysKey == syskey);

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

        private TNC_TransactionTypeEntity GetDefaultModel()
        {
            return new TNC_TransactionTypeEntity { IsActive = true };
        }

        #endregion
    }
}
