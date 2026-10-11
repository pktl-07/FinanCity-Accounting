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

    [RoutePrefix("CashbookTransfer")]
    public class CashbookTransferController : BaseApiController
    {
        #region Fields

        CashBookTransfer _cashbookTransfer;
        KendoGridFilterTransalator _kendoGridFilter;
        CashbookTransferValidator _validator;

        #endregion

        #region Ctor

        public CashbookTransferController(CashBookTransfer cashbookTransfer,
            CashbookTransferValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
         {
            _validator = validator;
            _cashbookTransfer = cashbookTransfer;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CashBookTransferEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashBookTransferEntity>(h);

                return _cashbookTransfer.GetPager(currentPage: 
                    currentPage, 
                    pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CashBookTransferEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashBookTransferEntity>(h);

                return _cashbookTransfer.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CashBookTransferEntity Find(string key)
        {
            try
            {
                return _cashbookTransfer.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CashBookTransferEntity Default()
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
        public IHttpActionResult Save(CashBookTransferEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _cashbookTransfer.InsertAsTransaction(ref model);

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
        public IHttpActionResult Update(CashBookTransferEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _cashbookTransfer.UpdateAsTransaction(model);
        
                    scope.Complete();
                }

                return Ok(_cashbookTransfer.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _cashbookTransfer.Delete(filter: f => f.SysKey == syskey);

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
        public IHttpActionResult Post(CashBookTransferEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _cashbookTransfer.PostAsTransaction(model);

                    scope.Complete();
                }
                return Ok(_cashbookTransfer.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _cashbookTransfer.ReverseAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_cashbookTransfer.Finds(filter:f=>f.SysKey == syskey).Single());
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
                    _cashbookTransfer.VoidAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_cashbookTransfer.Finds(filter: f => f.SysKey == syskey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("recap/{parentId}")]
        public IEnumerable<BTableEntity> Recap(string parentId)
        {
            try
            {
                return _cashbookTransfer.RecapAsTransaction(parentId: parentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utility

        private CashBookTransferEntity GetDefaultModel()
        {
            return new CashBookTransferEntity
            {

            };
        }

        #endregion
    }
}
