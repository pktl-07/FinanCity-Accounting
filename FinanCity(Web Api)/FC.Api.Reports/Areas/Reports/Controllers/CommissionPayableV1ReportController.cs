namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data.Extensions;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CommissionPayableV1Report")]
    public class CommissionPayableV1ReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private FinancialReport _financialReport;

        #endregion

        #region Ctor

        public CommissionPayableV1ReportController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<CommissionPayableReportFilter>(h);

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
        public CommissionPayableReportFilter Default()
        {
            try
            {
                return new CommissionPayableReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CommissionPayableReportFilter, bool>> filter)
        {
            try
            {
                var ds = new DataSet();

                var header = _financialReport.PayableHeaderDataSet(filter: filter);

                filter = filter.Constant((x) => x.PaidStatus == (int)SalesViewType.Pending);
                var detail = _financialReport.PayableDetailDataSet(filter: filter);

                ds.Tables.Add(header);
                ds.Tables.Add(detail);

                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "AP_002.rpt";
                _crystal.Configurator.Attachment.Title = "Commission Payable By Individual SP";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CommissionPayableReportFilter.SalesDate)}"];
                _crystal.SetFormulaField("TitleFor", "Commission Payable (By Individual SP)");
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CommissionPayableReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("SalespersonFor", formulaFields[$"{nameof(CommissionPayableReportFilter.SPSK)}"]);
                _crystal.SetFormulaField("SalespersonTypeFor", formulaFields[$"{nameof(CommissionPayableReportFilter.SalespersonType)}"]);
                _crystal.SetFormulaField("StatusFor", SalesViewType.Pending.ToDescription());

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
