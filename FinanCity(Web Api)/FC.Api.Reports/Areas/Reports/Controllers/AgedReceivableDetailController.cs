namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;

    [RoutePrefix("AgedReceivableDetail")]
    public class AgedReceivableDetailController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private AgedReceivables _agedReceivable;

        #endregion

        #region Ctor

        public AgedReceivableDetailController(ICrystalReporter crystal,
          AgedReceivables agedReceivable,
          DownloadResult downloadResult,
          KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _crystal = crystal;
            _kendoGridFilter = kendoGridFilter;
            _agedReceivable = agedReceivable;
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
                var e = _kendoGridFilter.Convert<AgedReceivableFilter>(h);

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
        public AgedReceivableFilter Default()
        {
            try
            {
                return new AgedReceivableFilter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoopulateRptDoc(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            try
            {
                var ds = _agedReceivable.AgedReceivableDetailReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_016.rpt";
                _crystal.Configurator.Attachment.Title = "Aged Receivables Detail";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(AgedReceivableFilter.SalesDate)}"];
                _crystal.SetFormulaField("DateFor", formulaFields[$"{nameof(AgedReceivableFilter.SalesDate)}"]);
                _crystal.SetFormulaField("ClientFor", formulaFields[$"{nameof(AgedReceivableFilter.ClientSK)}"]);

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
