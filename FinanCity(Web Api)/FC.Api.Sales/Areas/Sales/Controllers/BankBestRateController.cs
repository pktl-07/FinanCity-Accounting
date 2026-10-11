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
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using FC.Api.SalesAreas.Areas.Validators;

    [RoutePrefix("BankBestRate")]
    public class BankBestRateController : BaseApiController
    {
        #region Fields

        BankBestRate _bankBestRate;
        KendoGridFilterTransalator _kendoGridFilter;
        BankBestRateValidator _validator;

        #endregion

        #region Ctor

        public BankBestRateController(BankBestRate bankBestRate,
            BankBestRateValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _bankBestRate = bankBestRate;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<BankBestRateEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<BankBestRateEntity>(h);

                return _bankBestRate.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<BankBestRateEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<BankBestRateEntity>(h);

                return _bankBestRate.Finds(filter: e)
                    .OrderBy(x => x.Rate);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public BankBestRateEntity Find(string key)
        {
            try
            {
                return _bankBestRate.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public BankBestRateEntity Default()
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
        public IHttpActionResult Save(BankBestRateEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _bankBestRate.Insert(model);

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
        public IHttpActionResult Update(BankBestRateEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _bankBestRate.Update(entity: model, filter: f => f.SysKey == model.SysKey);
                    scope.Complete();
                }

                return Ok(_bankBestRate.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _bankBestRate.Delete(filter: f => f.SysKey == syskey);

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

        private BankBestRateEntity GetDefaultModel()
        {
            return new BankBestRateEntity
            {
                SysKey = GeneralUtility.Key,
                IsActive = true
            };
        }

        #endregion
    }
}
