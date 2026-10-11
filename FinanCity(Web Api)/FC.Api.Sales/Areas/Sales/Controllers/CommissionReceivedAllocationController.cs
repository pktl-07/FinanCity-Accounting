namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("CommissionReceivedAllocation")]
    public class CommissionReceivedAllocationController : BaseApiController
    {
        #region Field

        CommissionAllocation _allocation;
        KendoGridFilterTransalator _kendoGridFilter;
        CommissionReceivedAllocationValidator _validator;

        #endregion

        #region Ctor

        public CommissionReceivedAllocationController(
            CommissionReceivedAllocationValidator validator,
            CommissionAllocation allocation,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _allocation = allocation;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CommissionReceivedAllocationDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionReceivedAllocationDTO>(h);

                return _allocation.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<AllocationEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AllocationEntity>(h);

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
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

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
        [Route("post")]
        public IHttpActionResult Post(AllocationEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _allocation.PostAsTransaction(entity: model, autoAllocate: false);

                    scope.Complete();
                }

                return Ok(model);
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
        [Route("update")]
        public IHttpActionResult Update(AllocationEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _allocation.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_allocation.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _allocation.ReverseAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_allocation.Finds(filter: f => f.SysKey == syskey).Single());
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
                    _allocation.VoidAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_allocation.Finds(filter: f => f.SysKey == syskey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("recap/{key}")]
        public IEnumerable<BTableEntity> Recap(string key)
        {
            try
            {
                return _allocation.RecapAsTransaction(key: key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utility

        private AllocationEntity GetDefaultModel()
        {
            return new AllocationEntity
            {
                GrossToPromote = true,
                AllocationDate = GeneralUtility.ServerDate,
                TransType = (int)TransType.AllocationCommission,
            };
        }

        #endregion
    }
}
