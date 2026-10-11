namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("SalesManagementReport")]
    public class SalesManagementReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesReport _salesReport;

        #endregion

        #region Ctor

        public SalesManagementReportController(ICrystalReporter crystal,
          SalesReport salesReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _salesReport = salesReport;
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
                var e = _kendoGridFilter.Convert<SalesSummaryReportFilter>(h);

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
        public SalesSummaryReportFilter Default()
        {
            try
            {
                return new SalesSummaryReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<SalesSummaryReportFilter, bool>> filter)
        {
            try
            {

                var ds = _salesReport.SalesSummaryReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_018.rpt";
                _crystal.Configurator.Attachment.Title = "Sales Management Report";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(SalesSummaryReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("InvoiceTitelFor", "Invoice No.");
                _crystal.SetFormulaField("InvDateTitleFor", "Invoice Date");
                _crystal.SetFormulaField("StatusTitleFor", "Receipt Status");

                _crystal.SetFormulaField("TitleFor", "Sales Management Report");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(SalesSummaryReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("StatusFor", formulaFields[$"{nameof(SalesSummaryReportFilter.ReceivedStatus)}"]);
                _crystal.SetFormulaField("TransactionTypeFor", formulaFields[$"{nameof(SalesSummaryReportFilter.TransactionType)}"]);
                _crystal.SetFormulaField("PropertyTypeFor", formulaFields[$"{nameof(SalesSummaryReportFilter.PropertyType)}"]);
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
