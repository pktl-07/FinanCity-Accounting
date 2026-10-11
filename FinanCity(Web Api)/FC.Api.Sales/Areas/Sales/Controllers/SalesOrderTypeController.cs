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
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("SalesOrderType")]
    public class SalesOrderTypeController : BaseApiController
    {
        #region Fields

        SalesOrderType _salesOrderType;
        KendoGridFilterTransalator _kendoGridFilter;
        SalesOrderTypeValidator _salesOrderTypeValidator;

        #endregion

        #region Ctor

        public SalesOrderTypeController(SalesOrderType salesOrderType,
            SalesOrderTypeValidator salesOrderTypeValidator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _salesOrderTypeValidator = salesOrderTypeValidator;
            _salesOrderType = salesOrderType;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<SalesOrderTypeDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<SalesOrderTypeDTO>(h);

                return _salesOrderType.GetPager(currentPage: currentPage, pageSize: pageSize, filter: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<SalesOrderTypeEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<SalesOrderTypeEntity>(h);

                return _salesOrderType.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public SalesOrderTypeEntity Find(string key)
        {
            try
            {
                return _salesOrderType.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public SalesOrderTypeEntity Default()
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
        public IHttpActionResult Save(SalesOrderTypeEntity model)
        {
            try
            {
                _salesOrderTypeValidator.ValidateAndThrow(model);
           
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _salesOrderType.Insert(model);

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
        public IHttpActionResult Update(SalesOrderTypeEntity model)
        {
            try
            {
                _salesOrderTypeValidator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _salesOrderType.Update(entity: model, filter: f => f.SysKey == model.SysKey);
                    scope.Complete();
                }

                return Ok(_salesOrderType.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _salesOrderType.Delete(filter: f => f.SysKey == syskey);

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

        private SalesOrderTypeEntity GetDefaultModel()
        {
            return new SalesOrderTypeEntity { IsActive = true };
        }

        #endregion
    }
}
