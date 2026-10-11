namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FluentValidation;
    using FC.Core;
    using FC.Api.AccountAreas.Models;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;

    [RoutePrefix("AccountPeriod")]
    public class AccountingPeriodController : BaseApiController
    {
        #region Fields

        private readonly AccountingPeriodUpdateValidator _updateValidator;
        private readonly AccountingPeriod _accountingPeriod;
        private readonly  KendoGridFilterTransalator _kendoGridFilter;
        private readonly AccountingPeriodValidator _validator;
        private readonly BatchModule _batchModule;

        #endregion

        #region Ctor

        public AccountingPeriodController(AccountingPeriod accountingPeriod,
            AccountingPeriodUpdateValidator updateValidator,
            AccountingPeriodValidator validator,
            BatchModule batchModule,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _updateValidator = updateValidator;
            _accountingPeriod = accountingPeriod;
            _kendoGridFilter = kendoGridFilter;
            _batchModule = batchModule;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<AccountingPeriodEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AccountingPeriodEntity>(h);

                return _accountingPeriod.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public AccountingPeriodEntity Find(string key)
        {
            try
            {
                return _accountingPeriod.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save()
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _accountingPeriod.Add12AccountingPeriod();
                    scope.Complete();
                }

                var lastAccountingYear = _accountingPeriod.LastAccountingYearCode();
                return Ok(_accountingPeriod.GetAccountingPeriodDTO(lastAccountingYear));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(AccountingPeriodUpdateModel model)
        {
            try
            {
                _updateValidator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var batchModule = _batchModule.Finds(filter: f => f.YC == model.YC && f.PC == model.PC && f.ModuleCode == model.ModuleCode).Single();
                    batchModule.EntryStatus = model.Status;

                    _batchModule.Update(batchModule, filter: f => f.SysKey == batchModule.SysKey);

                    scope.Complete();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
