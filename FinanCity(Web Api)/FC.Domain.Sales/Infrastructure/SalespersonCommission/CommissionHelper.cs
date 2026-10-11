namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Linq;
    using Ninject;
    using FC.Core;
    using FC.Core.Infrastructure;
    using FC.Domain.Sales.Business;
  
    public class CommissionHelper
    {
        #region Field

        private Employment _employment;
        private OverridingProfile _overridingProfile;
        private CommissionStructureDetail _schemeDetail;

        #endregion

        #region Ctor

        public CommissionHelper(Employment employment,
            OverridingProfile overridingProfile,
            CommissionStructureDetail schemeDetail)
        {
            _employment = employment;
            _overridingProfile = overridingProfile;
            _schemeDetail = schemeDetail;
        }

        public CommissionHelper() { }

        #endregion

        #region Methods

        public decimal GetSalesCommissionToDate(string spsk)
        {
            try
            {
                Guard.IsNotNullOrEmpty(spsk, "spsk");

                return CreateFactory(spsk)
                    .GetSalesCommissionToDate();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesCommissionToYear(string spsk, int? effectiveDate = null)
        {
            try
            {
                var f = CreateFactory(spsk: spsk, startEffectiveDate: effectiveDate);
                return f.GetSalesCommissionToYear();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public decimal GetSalesValueToDate(string spsk)
        {
            return CreateFactory(spsk).GetSalesValueToDate();
        }

        public decimal GetSalesValueToYear(string spsk, int? effectiveDate = null)
        {
            var f = CreateFactory(spsk: spsk, startEffectiveDate: effectiveDate);
            return f.GetSalesValueToYear();
        }

        public decimal FindGrossToNextPromote(string spsk)
        {
            try
            {
                return CreateFactory(spsk).FindGrossToNextPromote();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetCommissionScheme(string spsk)
        {
            try
            {

                var emp = _employment.Finds(filter: f => f.SysKey == spsk).Single();
                var sd = _schemeDetail.Finds(filter: f => f.SysKey == emp.SchemeDetailSysKey).Single();
                var fd = _schemeDetail.Finds(filter: f => f.ParentSK == sd.ParentSK).OrderBy(x => x.MinCom).First();

                emp.SchemeDetailSysKey = fd.SysKey;
                emp.OpeningAccSales = 0;

                _employment.Update(emp,
                    filter: f => f.SysKey == emp.SysKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetCloseSalesToDate(string spsk, string invoicesk)
        {
            var employee = _employment.Finds(filter: f => f.SysKey == spsk).First();
            var op = employee.OpeningAccSales;

            var od = _overridingProfile.Finds(filter: f => f.SPSK == spsk && f.PTOSK == invoicesk);
            var resetDate = od.Last().ResetCommissionEffectiveStartDate;

            if (resetDate != null)
            {
                DI.Current.Rebind<ICommissionBaseHelperFactory>()
                    .To<SelespersonWhoHasResetScheme>()
                    .WithConstructorArgument("spsk", spsk)
                    .WithConstructorArgument("effectiveDate", resetDate);

                return op + DI.Current.Get<SelespersonWhoHasResetScheme>().GetSalesCommissionToYear();
            }

            DI.Current.Rebind<ICommissionBaseHelperFactory>()
                  .To<SalespersonWhoDoNotHaveResetScheme>()
                  .WithConstructorArgument("spsk", spsk);

            return op + DI.Current.Get<SalespersonWhoDoNotHaveResetScheme>().GetSalesCommissionToDate();
        }

        #endregion

        #region Utilities

        private ICommissionBaseHelperFactory CreateFactory(string spsk,
            int? startEffectiveDate = null)
        {
            try
            {
                var employee = _employment.Finds(filter: f => f.SysKey == spsk).LastOrDefault();
            
                if (startEffectiveDate == null && employee != null)
                    startEffectiveDate = employee.ResetCommissionEffectiveStartDate;
              
                if (startEffectiveDate != null &&
                    startEffectiveDate.Value <= GeneralUtility.ServerDate)
                {
                    DI.Current.Rebind<SelespersonWhoHasResetScheme>()
                    .To<SelespersonWhoHasResetScheme>()
                    .WithConstructorArgument("spsk", spsk)
                    .WithConstructorArgument("effectiveDate", startEffectiveDate);

                    return DI.Current.Get<SelespersonWhoHasResetScheme>();
                }

                DI.Current.Rebind<SalespersonWhoDoNotHaveResetScheme>()
                 .To<SalespersonWhoDoNotHaveResetScheme>()
                 .WithConstructorArgument("spsk", spsk);

                return DI.Current.Get<SalespersonWhoDoNotHaveResetScheme>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
