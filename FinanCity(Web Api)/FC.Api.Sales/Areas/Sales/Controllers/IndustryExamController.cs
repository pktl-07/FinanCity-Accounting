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
    using FC.Domain.Sales.Entity;

    [RoutePrefix("IndustryExam")]
    public class IndustryExamController : BaseApiController
    {
        #region Fields

        IndustryExam _industryExam;
        KendoGridFilterTransalator _kendoGridFilter;
        IndustryExamValidator _validator;

        #endregion

        #region Ctor

        public IndustryExamController(IndustryExam industryExam,
            IndustryExamValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _industryExam = industryExam;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<IndustryExamEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<IndustryExamEntity>(h);

                return _industryExam.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<IndustryExamEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<IndustryExamEntity>(h);

                return _industryExam.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public IndustryExamEntity Find(string key)
        {
            try
            {
                return _industryExam.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public IndustryExamEntity Default()
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
        public IHttpActionResult Save(IndustryExamEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    model = _industryExam.Insert(model);

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
        public IHttpActionResult Update(IndustryExamEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _industryExam.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();

                }

                return Ok(_industryExam.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _industryExam.Delete(filter: f => f.SysKey == syskey);

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

        private IndustryExamEntity GetDefaultModel()
        {
            return new IndustryExamEntity { IsActive = true };
        }

        #endregion
    }
}
