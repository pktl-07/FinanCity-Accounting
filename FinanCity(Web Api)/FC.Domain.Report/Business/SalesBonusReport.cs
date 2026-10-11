namespace FC.Domain.Report.Business
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Report.DAO;
    using FC.Domain.Report.Filter;
    using FC.Domain.Sales.Business;

    public class SalesBonusReport : BusinessLogic<SalesBonusReportDAO>
    {
        #region Fields

        private CareerManagement _careerManagement;
        private Person _person;

        #endregion

        #region Ctor

        public SalesBonusReport(CareerManagement careerManagement,
            Person person)
        {
            _careerManagement = careerManagement;
            _person = person;
        }

        #endregion

        #region Methods

        public DataTable RecruitmentIncentiveReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                return DAO.RecruitmentIncentiveReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RecruitmentIncentivePayableReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                return DAO.RecruitmentIncentivePayableReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RecruitmentIncentivePaidReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                return DAO.RecruitmentIncentivePaidReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CareerManagementSummaryDataTable(Expression<Func<SalesBonusReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;
                string spsk = null;
                int? rank = null;

                filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(SalesBonusReportFilter.SalesDate)}")
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);

                    if (mem == $"{nameof(SalesBonusReportFilter.T1)}")
                        if (spsk == null)
                            spsk = value;

                    if (mem == $"{nameof(SalesBonusReportFilter.Rank)}")
                        if (rank == null)
                            rank = int.Parse(value);
                });

                var d = _careerManagement.GetCareerManagementDownTree(date: toDate.Value);

                if (rank != null)
                    d = d.Where(x => x.Rank == rank.Value);

                if (spsk != null)
                    d = d.Where(x => x.SPSK == spsk);

                return d.Select(x => new
                {
                    SPSK = x.SPSK,
                    Name = x.Name,
                    NRIC = x.NRIC,
                    Rank = x.Rank,
                    RankName = ((RankType)x.Rank).ToDescription(),
                })
                    .OrderByDescending(x => x.Rank)
                    .ToDataTable("CareerManagementSummaryDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SalesBonusDetailReport(Expression<Func<SalesBonusReportFilter, bool>> filter)
        {
            try
            {
                return DAO.SalesBonusDetailReport(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
