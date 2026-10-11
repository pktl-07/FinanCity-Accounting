namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CashbookPaymentReport")]
    public class CashbookPaymentReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private CashbookReport _cashbookReport;

        #endregion

        #region Ctor

        public CashbookPaymentReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<CashbookReceiptPaymentReportFilter>(h);

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
        public CashbookReceiptPaymentReportFilter Default()
        {
            try
            {
                return new CashbookReceiptPaymentReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CashbookReceiptPaymentReportFilter, bool>> filter)
        {
            try
            {
                //filter = filter.Constant(f => f.TransType == (int)TransType.CashBookPayment);
                var ds = _cashbookReport.CashbookPaymentReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC008.rpt";
                _crystal.Configurator.Attachment.Title = "Payment Log";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CashbookReceiptPaymentReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("TitleFor", "Payment Log");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CashbookReceiptPaymentReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("CashbookFor", formulaFields[$"{nameof(CashbookReceiptPaymentReportFilter.CBSK)}"]);
                _crystal.SetFormulaField("TransDateFor", "Payment Date");
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
