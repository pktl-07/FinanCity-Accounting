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

    [RoutePrefix("CommissionPaidReport")]
    public class CommissionPaidReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private FinancialReport _financialReport;

        #endregion

        #region Ctor

        public CommissionPaidReportController(ICrystalReporter crystal,
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
                filter = filter.Constant((x) => x.PaidStatus == (int)SalesViewType.Close);
                var detail = _financialReport.PayableDetailDataSet(filter: filter);

                int? fromPaidDate = null;
                int? toPaidDate = null;

                filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionPayableReportFilter.PaidDate)}")
                    {
                        if (fromPaidDate == null)
                            fromPaidDate = int.Parse(value);
                        else if (toPaidDate == null)
                            toPaidDate = int.Parse(value);
                    }
                });

                var headerFilter = filter.Erase($"{nameof(CommissionPayableReportFilter.PaidDate)}");
                headerFilter = headerFilter.Constant(x => x.SalesDate <= toPaidDate && x.SalesDate >= fromPaidDate);
                var header = _financialReport.PayableHeaderDataSet(filter: headerFilter);

                ds.Tables.Add(header);
                ds.Tables.Add(detail);

                var formulaFields = filter.FormulaFieldFor();
                var title = "Salesperson Commission Paid";

                _crystal.Configurator.RptFileName = "AP_001.rpt";
                _crystal.Configurator.Attachment.Title = title;
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(CommissionPayableReportFilter.PaidDate)}"];

                _crystal.SetFormulaField("TitleFor", title);
                _crystal.SetFormulaField("DateRangeFor", formulaFields[$"{nameof(CommissionPayableReportFilter.PaidDate)}"]);
                _crystal.SetFormulaField("SalespersonFor", formulaFields[$"{nameof(CommissionPayableReportFilter.SPSK)}"]);
                _crystal.SetFormulaField("SalespersonTypeFor", formulaFields[$"{nameof(CommissionPayableReportFilter.SalespersonType)}"]);
                _crystal.SetFormulaField("StatusFor", SalesViewType.Close.ToDescription());
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
