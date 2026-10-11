namespace FC.Api.AccountAreas.Areas.Controllers
{
    using System;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Accounts.Business;
 
    [RoutePrefix("AccountYearClosing")]
    public class AccountingYearClosingController : BaseApiController
    {
        #region Fields

        private readonly KendoGridFilterTransalator _kendoGridFilter;
        private readonly YearClosing _yearClosing;

        #endregion

        #region Ctor

        public AccountingYearClosingController(
            YearClosing yearClosing,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _yearClosing = yearClosing;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post()
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _yearClosing.CloseCurrentAccountingYear();
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
