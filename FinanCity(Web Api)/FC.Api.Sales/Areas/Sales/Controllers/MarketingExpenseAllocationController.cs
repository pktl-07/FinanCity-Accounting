namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data.Extensions;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("MarketingExpenseAllocation")]
    public class MarketingExpenseAllocationController : BaseApiController
    {
        #region Fields

        private CommissionAllocation _allocation;
        private KendoGridFilterTransalator _kendoGridFilter;
        private MarketingPromotionAllocationValidator _validator;

        #endregion

        #region Ctor

        public MarketingExpenseAllocationController(CommissionAllocation allocation,
            MarketingPromotionAllocationValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _kendoGridFilter = kendoGridFilter;
            _allocation = allocation;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<AllocationEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AllocationEntity>(h);
                e = e.Constant((x) => x.TransType == (int)TransType.AllocationMarketingExpense);

                return _allocation.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public AllocationEntity Find(string key)
        {
            try
            {
                return _allocation.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public AllocationEntity Default()
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
        public IHttpActionResult Save(AllocationEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _allocation.Insert(model);
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
        public IHttpActionResult Update(AllocationEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _allocation.Update(model, filter: f => f.SysKey == model.SysKey); 
                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.SysKey));
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
                    _allocation.Delete(filter: f => f.SysKey == syskey);

                    scope.Complete();
                }
                return Ok(GetDefaultModel());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(AllocationEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.RecordStatus = (int)RecordStatus.Post;
                 
                    _allocation.Update(entity: model, filter: f => f.SysKey == model.SysKey);
                    
                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("reverse")]
        public IHttpActionResult Reverse(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var allocation = _allocation.Finds(filter: f => f.SysKey == syskey).SingleOrDefault();
                    allocation.RecordStatus = (int)RecordStatus.Update;

                    _allocation.Update(allocation, filter: f => f.SysKey == syskey);
                    scope.Complete();
                }

                return Ok(GetDefaultModel(syskey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("void")]
        public IHttpActionResult Void(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var allocation = _allocation.Finds(filter: f => f.SysKey == syskey).SingleOrDefault();
                    allocation.RecordStatus = (int)RecordStatus.Void;

                    _allocation.Update(allocation, filter: f => f.SysKey == syskey);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(syskey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private AllocationEntity GetDefaultModel(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new AllocationEntity { TransType = (int)TransType.AllocationMarketingExpense };

            return _allocation.Finds(filter: f => f.SysKey == key).SingleOrDefault();
        }

        #endregion
    }
}
