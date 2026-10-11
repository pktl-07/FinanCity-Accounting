namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("SalesDetailReport")]
    public class SalesDetailReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesReport _salesReport;

        #endregion

        #region Ctor

        public SalesDetailReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<SalesDetailReportFilter>(h);

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
        public SalesDetailReportFilter Default()
        {
            try
            {
                return new SalesDetailReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<SalesDetailReportFilter, bool>> filter)
        {
            try
            {
                var ds = _salesReport.SalesDetailReport(
                    summaryFilter: filter.To<SalesDetailReportFilter, SalesSummaryReportFilter>(),
                    detailFilter: filter.To<SalesDetailReportFilter, IndividualSalesReportFilter>());

                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_007.rpt";
                _crystal.Configurator.Attachment.Title = "Sales Detail";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(SalesDetailReportFilter.SalesDate)}"];
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(SalesDetailReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("ReceivedStatusFor", formulaFields[$"{nameof(SalesDetailReportFilter.ReceivedStatus)}"]);
                _crystal.SetFormulaField("PayableStatusFor", formulaFields[$"{nameof(SalesDetailReportFilter.PaidStatus)}"]);
               
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
