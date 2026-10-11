namespace FC.Api.Controllers
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Business;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Core.Data.Extensions;

    [RoutePrefix("DataList")]
    public class DataListController : BaseApiController
    {
        #region Fields

        private AccountSetup _accountSetup;
        private BTable _journalLedger;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DataList _business;
        private IFinanCityIdentity _identity;

        #endregion

        #region Ctor

        public DataListController(
            BTable journalLedger,
            DataList business,
            IFinanCityIdentity identity,
            KendoGridFilterTransalator kendoGridFilter,
            AccountSetup accountSetup)
        {

            _identity = identity;
            _journalLedger = journalLedger;
            _business = business;
            _kendoGridFilter = kendoGridFilter;
            _accountSetup = accountSetup;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("GetSalespersonCommissionPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<SalespersonCommissionDTO> GetSalespersonCommissionPager(int currentPage = 0, 
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<SalespersonCommissionDTO>(h);

                if (_identity.AccountType == (int)UserAccountType.SalespersonAccount)
                {
                    var spsk = _identity.SPSK;
                    e = e.Constant(x => x.SPSK == spsk);
                }

                return _business.GetSalespersonCommissionPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetChartOfAccounts/{yc}/{accountPrefix}")]
        public IEnumerable<ChartOfAccountDTO> GetChartOfAccounts(string yc, string accountPrefix)
        {
            try
            {
                return _accountSetup.FindCOATable(yc: yc,
                    accountPrefix: accountPrefix);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetJournalLedgerSummary")]
        public IEnumerable<JournalSummaryDTO> GetSummary()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<JournalSummaryDTO>(h);

                return _journalLedger.GetSummary(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("ClientAndLeadPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<LeadAndContactDTO> GetClientAndLeadPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadAndContactDTO>(h);

                return _business.GetClientAndLeadPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetLeadAndSalesperson/{currentPage}/{pageSize}")]
        public IDataPagerResult<LeadAndSalespersonDTO> GetLeadAndSalesperson(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadAndSalespersonDTO>(h);

                return _business.GetLeadAndSalesperson(currentPage: currentPage,
                    pageSize: pageSize,
                    where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetAuditPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<JournalSummaryDTO> GetAuditPager(int currentPage = 0,
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<JournalSummaryDTO>(h);

                return _journalLedger.GetAuditDataPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        #endregion
    }
}
