namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("SalesBonusReport")]
    public class SalesBonusReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesBonusReport _salesBonusReport;

        #endregion

        #region Ctor

        public SalesBonusReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<SalesBonusReportFilter>(h);

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

        private void PoopulateRptDoc(Expression<Func<SalesBonusReportFilter, bool>> filter)
        {
            try
            {
                var ds = new DataSet();

                var summary = _salesBonusReport.CareerManagementSummaryDataTable(filter: filter);
                var detail = _salesBonusReport.SalesBonusDetailReport(filter: filter);

                ds.Tables.Add(summary);
                ds.Tables.Add(detail);

                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_020.rpt";
                _crystal.Configurator.Attachment.Title = "Sales Bonus Report";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(SalesBonusReportFilter.SalesDate)}"];

                _crystal.SetFormulaField("RankFor", formulaFields[$"{nameof(SalesBonusReportFilter.Rank)}"]);
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(SalesBonusReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("SalepersonFor", formulaFields[$"{nameof(SalesBonusReportFilter.T1)}"]);

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
