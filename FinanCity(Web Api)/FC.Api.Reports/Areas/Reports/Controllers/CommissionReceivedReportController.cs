namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CommissionReceivedReport")]
    public class CommissionReceivedReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private FinancialReport _financialReport;

        #endregion

        #region Ctor

        public CommissionReceivedReportController(ICrystalReporter crystal,
          FinancialReport financialReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _financialReport = financialReport;
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
                var e = _kendoGridFilter.Convert<CommissionReceivedReportFilter>(h);

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
        public CommissionReceivedReportFilter Default()
        {
            try
            {
                return new CommissionReceivedReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CommissionReceivedReportFilter, bool>> filter)
        {
            try
            {
                var ds = _financialReport.CommissionReceivedReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_010.rpt";
                _crystal.Configurator.Attachment.Title = "Commission Received";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CommissionReceivedReportFilter.SalesDate)}"];
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CommissionReceivedReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("SalespersonFor", formulaFields[$"{nameof(CommissionReceivedReportFilter.SPSK)}"]);
                _crystal.SetFormulaField("CashbookFor", formulaFields[$"{nameof(CommissionReceivedReportFilter.CBSK)}"]);
           
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
