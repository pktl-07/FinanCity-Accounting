namespace FC.Api.AccountAreas.Areas.Controllers
{
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("Job")]
    public class JobController : BaseApiController
    {
        #region Fields

        Job _job;
        KendoGridFilterTransalator _kendoGridFilter;
        CashbookTransferValidator _validator;

        #endregion

        #region Ctor

        public JobController(Job job,
            CashbookTransferValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _validator = validator;
            _job = job;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<JobEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<JobEntity>(h);

                return _job.GetPager(currentPage:
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
        public IEnumerable<JobEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<JobEntity>(h);

                return _job.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public JobEntity Find(string key)
        {
            try
            {
                return _job.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public JobEntity Default()
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
        public IHttpActionResult Save(JobEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _job.Insert(model);

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
        public IHttpActionResult Update(JobEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _job.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_job.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _job.Delete(filter: f => f.SysKey == syskey);

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

        private JobEntity GetDefaultModel()
        {
            return new JobEntity
            {
                IsActive = true,
            };
        }

        #endregion
    }
}
