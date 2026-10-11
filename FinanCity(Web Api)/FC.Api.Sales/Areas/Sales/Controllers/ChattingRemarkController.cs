namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("ChattingRemark")]
    public class ChattingRemarkController : BaseApiController
    {
        #region Fields

        Chatting _chattingRemark;
        KendoGridFilterTransalator _kendoGridFilter;
 
        #endregion

        #region Ctor

        public ChattingRemarkController(Chatting chattingRemark,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _chattingRemark = chattingRemark;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<ChattingEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<ChattingEntity>(h);

                return _chattingRemark.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public ChattingEntity Find(string key)
        {
            try
            {
                return _chattingRemark.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(ChattingEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _chattingRemark.InsertAsTransaction(entity: model);

                    scope.Complete();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private ChattingEntity GetDefaultModel()
        {
            return new ChattingEntity();
        }

        #endregion
    }
}
