namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("RecruitmentIncentivePayableReport")]
    public class RecruitmentIncentivePayableReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesBonusReport _salesBonusReport;

        #endregion

        #region Ctor

        public RecruitmentIncentivePayableReportController(ICrystalReporter crystal,
          SalesBonusReport salesBonusReport,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _salesBonusReport = salesBonusReport;
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
                var e = _kendoGridFilter.Convert<RecruitmentIncentiveReportFilter>(h);

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
        public RecruitmentIncentiveReportFilter Default()
        {
            try
            {
                return new RecruitmentIncentiveReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {

                var ds = _salesBonusReport.RecruitmentIncentivePayableReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_013.rpt";
                _crystal.Configurator.Attachment.Title = "Recruitment Incentive Payable Report";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(RecruitmentIncentiveReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("ReportTitle", "Recruitment Incentive Payable Report");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(RecruitmentIncentiveReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("SalepersonFor", formulaFields[$"{nameof(RecruitmentIncentiveReportFilter.SPSK)}"]);
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
