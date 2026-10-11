namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("BranchOut")]
    public class BranchOutController : BaseApiController
    {
        #region Fields

        CareerManagement _careerManagement;
        KendoGridFilterTransalator _kendoGridFilter;
        CareerManagementValidator _validator;

        #endregion

        #region Ctors

        public BranchOutController(CareerManagement careerManagement,
            CareerManagementValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _careerManagement = careerManagement;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CareerManagementDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CareerManagementDTO>(h);

                e = e.Constant((x) => x.TransType == (int)TransType.BranchOut
                && x.IsDefault == false);

                return _careerManagement.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CareerManagementEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CareerManagementEntity>(h);

                return _careerManagement.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CareerManagementEntity Find(string key)
        {
            try
            {
                return _careerManagement.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CareerManagementEntity Default()
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
        public IHttpActionResult Save(CareerManagementEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var last = _careerManagement.Finds(filter: f => f.SPSK == model.SPSK
                    && f.TransType == (int)TransType.ChangeOfStatus)
                        .OrderBy(x => x.Auto)
                        .LastOrDefault();

                    if (last != null)
                    {
                        model.BonusPercent = last.BonusPercent;
                        model.ReasonToTerminate = last.ReasonToTerminate;
                        model.IsActive = last.IsActive;
                        model.Rank = last.Rank;
                    }

                    model = _careerManagement.InsertAsTransaction(entity: model);
                    scope.Complete();

                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private CareerManagementEntity GetDefaultModel()
        {
            return new CareerManagementEntity
            {
                TransType = (int)TransType.BranchOut,
            };
        }

        #endregion
    }
}
