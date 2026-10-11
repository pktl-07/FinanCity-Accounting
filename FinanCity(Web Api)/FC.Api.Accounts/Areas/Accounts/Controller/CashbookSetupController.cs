namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;
    using FluentValidation;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;

    [RoutePrefix("CashbookSetup")]
    public class CashbookSetupController : BaseApiController
    {
        #region Fields

        CashbookSetup _cashBookSetup;
        KendoGridFilterTransalator _kendoGridFilter;
        CashbookSetupValidator _cashBookSetupValidator;
        AccountSetup _accountSetup;

        #endregion

        #region Ctor

        public CashbookSetupController(CashbookSetup cashBookSetup,
            CashbookSetupValidator cashBookSetupValidator,
            AccountSetup accountSetup,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _accountSetup = accountSetup;
            _cashBookSetupValidator = cashBookSetupValidator;
            _cashBookSetup = cashBookSetup;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CashbookSetupDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashbookSetupDTO>(h);

                return _cashBookSetup.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CashbookSetupEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashbookSetupEntity>(h);

                return _cashBookSetup.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CashbookSetupEntity Find(string key)
        {
            try
            {
                return _cashBookSetup.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CashbookSetupEntity Default()
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
        public IHttpActionResult Save(CashbookSetupEntity model)
        {
            try
            {
                _cashBookSetupValidator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _cashBookSetup.Insert(model);
                    
                    SyncUpdateOnAccount(model);
                    
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
        public IHttpActionResult Update(CashbookSetupEntity model)
        {
            try
            {
                _cashBookSetupValidator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _cashBookSetup.Update(entity: model, filter: f => f.SysKey == model.SysKey);
                   
                    SyncUpdateOnAccount(model);
                    
                    scope.Complete();
                }

                return Ok(_cashBookSetup.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _cashBookSetup.Delete(filter: f => f.SysKey == syskey);

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

        private CashbookSetupEntity GetDefaultModel()
        {

            var di = DI.Current.Get<IEssAppRepository>();

            return new CashbookSetupEntity
            {
                CURRSK = di.CURRSK,
                IsActive = true,
                CBType = (int)CashBookType.Bank,
            };
        }

        private void SyncUpdateOnAccount(CashbookSetupEntity entity)
        {
            try
            {
                var accsk = entity.AccSK;
                var acc = _accountSetup.Finds(filter: f => f.SysKey == entity.AccSK).Single();

                _accountSetup.Update(selector: s => new AccountSetupEntity
                {
                    AccCode = acc.AccCode,
                    AccDescription = entity.CashbookName,
                }, filter: f => f.SysKey == accsk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
