namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CompareIncomeStatementMonthlyReport")]
    public class CompareIncomeStatementMonthlyReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private AccountingReport _accountingReport;

        #endregion

        #region Ctor

        public CompareIncomeStatementMonthlyReportController(
          ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<CompareMonthlyIncomeStatementFilter>(h);

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
        public CompareMonthlyIncomeStatementFilter Default()
        {
            try
            {
                return new CompareMonthlyIncomeStatementFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CompareMonthlyIncomeStatementFilter, bool>> filter)
        {
            try
            {
                var ds = _accountingReport.IncomeStatementMonthlyReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC020.rpt";
                _crystal.Configurator.Attachment.Title = "Monthly Compare Statement";
                _crystal.SetFormulaField("YearCodeFor", formulaFields[$"{nameof(CompareYearlyIncomeStatementFilter.YCStart)}"]);           
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
