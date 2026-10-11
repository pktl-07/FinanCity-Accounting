namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("CommissionPayableBS")]
    public class CommissionPayableBSController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private FinancialReport _financialReport;

        #endregion

        #region Ctor

        public CommissionPayableBSController(ICrystalReporter crystal,
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
                var e = _kendoGridFilter.Convert<CommissionPayableBSReportFilter>(h);

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
        public CommissionPayableBSReportFilter Default()
        {
            try
            {
                return new CommissionPayableBSReportFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<CommissionPayableBSReportFilter, bool>> filter)
        {
            try
            {
                var ds = _financialReport.CommissionPayableBSReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();
                var title = "Account Payable Report [BS]";

                _crystal.Configurator.RptFileName = "AP_004.rpt";
                _crystal.Configurator.Attachment.Title = "Salesperson Commission Payable BS";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CommissionPayableBSReportFilter.SalesDate)}"];

                int? startDate = null;
                int? endDate = null;
                filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionPayableBSReportFilter.SalesDate)}")
                    {
                        if (startDate == null)
                            startDate = int.Parse(value);
                        else if (endDate == null)
                            endDate = int.Parse(value);
                    }
                });

                _crystal.SetFormulaField("TitleFor", title);
                _crystal.SetFormulaField("OpeningBalanceFor", _financialReport.OpeningPayableBalance(startDate: startDate.Value).ToString("N2"));
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CommissionPayableBSReportFilter.SalesDate)}"]);
                _crystal.SetFormulaField("SalespersonFor", formulaFields[$"{nameof(CommissionPayableBSReportFilter.SPSK)}"]);
                _crystal.SetFormulaField("SalespersonTypeFor", formulaFields[$"{nameof(CommissionPayableBSReportFilter.SalespersonType)}"]);

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
