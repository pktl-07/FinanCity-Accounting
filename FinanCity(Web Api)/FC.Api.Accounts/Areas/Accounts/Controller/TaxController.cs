namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FluentValidation;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;

    [RoutePrefix("Tax")]
    public class TaxController : BaseApiController
    {
        #region Fields

        private Tax _business;
        private KendoGridFilterTransalator _kendoGridFilter;
        private TaxValidator _validator;

        #endregion

        #region Ctor

        public TaxController(Tax business,
            KendoGridFilterTransalator kendoGridFilter,
            TaxValidator validator)
        {
            _business = business;
            _kendoGridFilter = kendoGridFilter;
            _validator = validator;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<TaxEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<TaxEntity>(h);
                
                return _business.GetPager(currentPage, pageSize,e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<TaxEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<TaxEntity>(h);

                return _business.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public TaxEntity Find(string key)
        {
            try
            {
                return _business.Finds(filter:f=>f.SysKey == key).SingleOrDefault() ?? new TaxEntity { SysKey = null , TaxPercent = 0 , TaxCode = "N/A" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public TaxEntity Default()
        {
            try
            {
                return new TaxEntity
                {
                    TaxType = (int)TaxType.Purchase,
                    TaxCategory = (int)TaxCategory.Standard,
                    IsActive = true,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(TaxEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                return Ok(_business.Insert(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(TaxEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                _business.Update(model, filter: f => f.SysKey == model.SysKey);

                return Ok(_business.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                _business.Delete(filter: f => f.SysKey == syskey);

                return Ok(new TaxEntity { IsActive = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
