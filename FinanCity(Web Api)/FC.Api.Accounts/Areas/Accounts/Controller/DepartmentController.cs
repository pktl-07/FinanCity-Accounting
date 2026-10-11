namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FluentValidation;
    using FC.Core.Data;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;

    [RoutePrefix("Department")]
    public class DepartmentController : BaseApiController
    {
        #region Fields

        private Department _business;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DepartmentSetupValidator _validator;
        private DepartmentSetupDeleteValidator _deleteValidator;

        #endregion

        #region Ctor

        public DepartmentController(Department business,
            DepartmentSetupDeleteValidator deleteValidator,
            DepartmentSetupValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _deleteValidator = deleteValidator;
            _validator = validator;
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<DepartmentEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<DepartmentEntity>(h);

                return _business.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<DepartmentEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<DepartmentEntity>(h);

                return _business.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public DepartmentEntity Find(string key)
        {
            try
            {
                return _business.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public DepartmentEntity Default()
        {
            try
            {
                return new DepartmentEntity
                {
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
        public IHttpActionResult Save(DepartmentEntity model)
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
        public IHttpActionResult Update(DepartmentEntity model)
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
                _deleteValidator.ValidateAndThrow(syskey);

                _business.Delete(filter: f => f.SysKey == syskey);

                return Ok(new DepartmentEntity { IsActive = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
