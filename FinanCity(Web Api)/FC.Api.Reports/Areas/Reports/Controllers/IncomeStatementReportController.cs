namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("IncomeStatementReport")]
    public class IncomeStatementReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private AccountingReport _accountingReport;

        #endregion

        #region Ctor

        public IncomeStatementReportController(ICrystalReporter crystal,
          AccountingReport accountingReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _accountingReport = accountingReport;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("Pdf")]
        public IHttpActionResult Pdf()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<AccountingReportFilter>(h);

                PoopulateRptDoc(filter: e);

                return _downloadResult.Excute(_crystal.ExportToStream(), Request,
                    _crystal.Configurator.Attachment.Title + ".pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public AccountingReportFilter Default()
        {
            try
            {
                return new AccountingReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<AccountingReportFilter, bool>> filter)
        {
            try
            {
                var beforeTax = "";

                var ds = _accountingReport.IncomeStatementReport(filter: filter, beforeTax: out beforeTax);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC005.rpt";
                _crystal.Configurator.Attachment.Title = "IncomeStatement";
                _crystal.Configurator.Attachment.ReportDate = "01";

                _crystal.SetFormulaField("DateRangeFor", $"{formulaFields[nameof(AccountingReportFilter.AccountingDate)]}");
                _crystal.SetFormulaField("TotalNetProfitFor", beforeTax);

                _crystal.SetDataSource(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
