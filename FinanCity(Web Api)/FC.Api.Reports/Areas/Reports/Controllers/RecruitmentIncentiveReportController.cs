namespace FC.Api.ReportAreas.Areas.Reports.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Domain.Report.Business;
    using FC.Domain.Report.Filter;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Infrastructure;

    [RoutePrefix("RecruitmentIncentiveReport")]
    public class RecruitmentIncentiveReportController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private DownloadResult _downloadResult;
        private SalesBonusReport _salesBonusReport;

        #endregion

        #region Ctor

        public RecruitmentIncentiveReportController(ICrystalReporter crystal,
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
                var ds = _salesBonusReport.RecruitmentIncentiveReport(filter: filter);
                var formulaFields = filter.FormulaFieldFor();

                _crystal.Configurator.RptFileName = "SA_019.rpt";
                _crystal.Configurator.Attachment.Title = "Recruitment Incentive Report";
                _crystal.Configurator.Attachment.ReportDate = formulaFields[$"{nameof(RecruitmentIncentiveReportFilter.SalesDate)}"];

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

    public abstract class DownStream
    {
        private CareerManagement _careerManagement;
        private SalespersonTree _salesPersonTree;
        private ManagerTree _managerTree;
        private DivisionDirectorTree _divisionDirectorTree;

        public DownStream(CareerManagement careerManagement,
            SalespersonTree salespersonTree,
            DivisionDirectorTree divisionDirectorTree,
            ManagerTree managerTree)
        {
            _careerManagement = careerManagement;
            _divisionDirectorTree = divisionDirectorTree;
            _salesPersonTree = salespersonTree;
            _managerTree = managerTree;
        }

        public IEnumerable<Context> Context(int date)
        {
            var _dto = _careerManagement.GetCareerManagementDownTree(date);

            DownStreamAdapter _adapter = null;

            _dto.Each(a =>
            {
                switch (a.Rank)
                {
                    case (int)RankType.SP:
                        _adapter = _salesPersonTree;
                        break;
                    case (int)RankType.TM:
                        _adapter = _managerTree;
                        break;
                    case (int)RankType.DD:
                        _adapter = _divisionDirectorTree;
                        break;
                }
               _adapter.Source = _dto;
            });
            return null;
        }
    }

    public abstract class DownStreamAdapter
    {
        public IEnumerable<SalesPersonTireDTO> Source { get; set; }

        protected IEnumerable<SalespersonRecursiveTree> CreateDownLine(string spsk)
        {
            return null;
        }

        public abstract IEnumerable<Context> CreateContext(string spsk);
    }

    public class ManagerTree : DownStreamAdapter
    {
        public override IEnumerable<Context> CreateContext(string spsk)
        {

            throw new NotImplementedException();
        }
    }

    public class SalespersonTree : DownStreamAdapter
    {
        public override IEnumerable<Context> CreateContext(string spsk)
        {
            throw new NotImplementedException();
        }
    }

    public class DivisionDirectorTree : DownStreamAdapter
    {
        public override IEnumerable<Context> CreateContext(string spsk)
        {
            throw new NotImplementedException();
        }
    }

    public class Context
    {
        public string SPSK { get; set; }
        public string Name { get; set; }
        public string L1 { get; set; }
        public string Rank { get; set; }
    }
}
