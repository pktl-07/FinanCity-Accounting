namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("IndividualSalesReport")]
    public class IndividualSalesReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesReport _salesReport;

        #endregion

        #region Ctor

        public IndividualSalesReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<IndividualSalesReportFilter>(h);

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
        public IndividualSalesReportFilter Default()
        {
            try
            {
                return new IndividualSalesReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<IndividualSalesReportFilter, bool>> filter)
        {
            try
            {
            
                var ds = _salesReport.IndividualSalesReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_009.rpt";
                _crystal.Configurator.Attachment.Title = "Individual Sales";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(IndividualSalesReportFilter.SalesDate)}"];
           
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(IndividualSalesReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("StatusFor", formulaFields[$"{nameof(IndividualSalesReportFilter.ReceivedStatus)}"]);
                _crystal.SetFormulaField("SalespersonFor", formulaFields[$"{nameof(IndividualSalesReportFilter.SPSK)}"]);
             
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
