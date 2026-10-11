namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CashbookLogReport")]
    public class CashbookLogReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private CashbookReport _cashbookReport;

        #endregion

        #region Ctor

        public CashbookLogReportController(ICrystalReporter crystal,
          CashbookReport cashbookReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _cashbookReport = cashbookReport;
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
                var e = _kendoGridFilter.Convert<CashbookLogReportFilter>(h);

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
        public CashbookLogReportFilter Default()
        {
            try
            {
                return new CashbookLogReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CashbookLogReportFilter, bool>> filter)
        {
            try
            {
                var openingBalance = 0.00M;
                var ds = _cashbookReport.CashbookLogReport(filter: filter, out openingBalance);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC009.rpt";
                _crystal.Configurator.Attachment.Title = "Cashbook Log";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CashbookLogReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("TitleFor", "Cashbook Log");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CashbookLogReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("CashbookFor", formulaFields[$"{nameof(CashbookLogReportFilter.CBSK)}"]);
                _crystal.SetFormulaField("OpeningTotalFor", openingBalance.ToString("N2"));
       
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
