namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core.Data;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("CurrencyRate")]
    public class CurrencyRateController : BaseApiController
    {
        #region Fields

        private Currency _currency;
        private CurrencyRate _currencyRate;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CurrencyRateValidator _validator;

        #endregion

        #region Ctor

        public CurrencyRateController(CurrencyRate currencyRate,
            Currency currency,
            CurrencyRateValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _currency = currency;
            _validator = validator;
            _currencyRate = currencyRate;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CurrencyRateDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CurrencyRateDTO>(h);

                return _currencyRate.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CurrencyRateDTO> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CurrencyRateDTO>(h);

                return _currencyRate.GetCurrencyRateDTO(e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CurrencyRateEntity Find(string key)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CurrencyRateEntity>(h);
              
                return _currencyRate.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CurrencyRateEntity Default()
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

        #endregion

        #region Utility

        private CurrencyRateEntity GetDefaultModel()
        {
            return new CurrencyRateEntity { Operator = "*" };
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(CurrencyRateEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                return Ok(_currencyRate.Insert(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CurrencyRateEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                _currencyRate.Update(model, filter: f => f.SysKey == model.SysKey);

                return Ok(_currencyRate.Finds(filter: f => f.SysKey == model.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
