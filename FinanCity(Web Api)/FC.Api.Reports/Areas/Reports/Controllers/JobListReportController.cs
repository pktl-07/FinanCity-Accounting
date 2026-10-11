namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("JobListReport")]
    public class JobListReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private JobReport _jobReport;

        #endregion

        #region Ctor

        public JobListReportController(ICrystalReporter crystal,
          JobReport jobReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _jobReport = jobReport;
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
                var e = _kendoGridFilter.Convert<JobActivityDatailReportFilter>(h);

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
        public JobActivityDatailReportFilter Default()
        {
            try
            {
                return new JobActivityDatailReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<JobActivityDatailReportFilter, bool>> filter)
        {
            try
            {
                var ds = _jobReport.JobActivityListReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "ACC010.rpt";
                _crystal.Configurator.Attachment.Title = "Job List";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CashbookReceiptPaymentReportFilter.SalesDate)}"];
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(JobActivityDatailReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("JobNoFor", formulaFields[$"{nameof(JobActivityDatailReportFilter.JobSK)}"]);
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
