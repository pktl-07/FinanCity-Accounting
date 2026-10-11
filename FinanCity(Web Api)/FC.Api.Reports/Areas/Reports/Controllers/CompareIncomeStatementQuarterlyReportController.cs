namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CompareIncomeStatementQuarterlyReport")]
    public class CompareIncomeStatementQuarterlyReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private AccountingReport _accountingReport;

        #endregion

        #region Ctor

        public CompareIncomeStatementQuarterlyReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<CompareQuarterlyIncomeStatementReportFilter>(h);

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
        public CompareQuarterlyIncomeStatementReportFilter Default()
        {
            try
            {
                return new CompareQuarterlyIncomeStatementReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CompareQuarterlyIncomeStatementReportFilter, bool>> filter)
        {
            try
            {
                var ds = _accountingReport.IncomeStatementQuarterlyReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC018.rpt";
                _crystal.Configurator.Attachment.Title = "Quarterly Compare Statement";

                _crystal.SetFormulaField("YearCodeFor", formulaFields[$"{nameof(CompareYearlyIncomeStatementFilter.YCStart)}"]);
                _crystal.SetFormulaField("PeriodOneFor", "First Quarter");
                _crystal.SetFormulaField("PeriodTwoFor", "Second Quarter");
                _crystal.SetFormulaField("PeriodThreeFor", "Third Quarter");
                _crystal.SetFormulaField("PeriodFourFor", "Forth Quarter");

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
