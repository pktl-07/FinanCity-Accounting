namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("PropertyTransactionListReport")]
    public class PropertyTransactionListReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesReport _salesReport;

        #endregion

        #region Ctor

        public PropertyTransactionListReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<PropertyTransactionListReportFilter>(h);

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
        public PropertyTransactionListReportFilter Default()
        {
            try
            {
                return new PropertyTransactionListReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<PropertyTransactionListReportFilter, bool>> filter)
        {
            try
            {

                var ds = _salesReport.PropertyTransactionListReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_018.rpt";
                _crystal.Configurator.Attachment.Title = "Sales Transaction List";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(PropertyTransactionListReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("InvoiceTitelFor", "Trans. No.");
                _crystal.SetFormulaField("InvDateTitleFor", "Trans. Date");
                _crystal.SetFormulaField("StatusTitleFor", "Status");

                _crystal.SetFormulaField("TitleFor", "Sales Transaction List");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(PropertyTransactionListReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("StatusFor", formulaFields[$"{nameof(PropertyTransactionListReportFilter.RecordStatus)}"]);
                _crystal.SetFormulaField("TransactionTypeFor", formulaFields[$"{nameof(PropertyTransactionListReportFilter.TransactionType)}"]);
                _crystal.SetFormulaField("PropertyTypeFor", formulaFields[$"{nameof(PropertyTransactionListReportFilter.PropertyType)}"]);
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
