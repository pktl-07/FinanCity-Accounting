namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
  
    public class SalespersonWhoDoNotHaveResetScheme : ICommissionBaseHelperFactory
    {
        #region Fields

        private string _spsk;
        private Person _person;
        private CommissionStructureDetail _commScheme;
        private CommissionAllocation _allocation;
        private Employment _employment;

        #endregion

        #region Ctor

        //this parameterless constructor is required for Ninject
        public SalespersonWhoDoNotHaveResetScheme() { }

        public SalespersonWhoDoNotHaveResetScheme(Person person,
            CommissionAllocation allocation,
            Employment employment,
            CommissionStructureDetail commissionScheme,
            string spsk)
        {
            _employment = employment;
            _allocation = allocation;
            _person = person;
            _commScheme = commissionScheme;
            _spsk = spsk;
        }

        #endregion

        #region Methods

        public decimal GetSalesCommissionToDate()
        {
            try
            {
                if (_spsk != null)
                    return _allocation.GetSalesCommissionToDate(_spsk);

                return 0.0M;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesCommissionToYear()
        {
            try
            {
                if (_spsk != null)
                {
                    var fromDate = int.Parse($"{GeneralUtility.ServerYear}0101");
                    var toDate = int.Parse($"{GeneralUtility.ServerYear}1231");
                    var result = _allocation.GetSalesCommissionToYear(spsk: _spsk,
                        fromDate: fromDate,
                        toDate: toDate);

                    return result;
                }

                return 0.0M;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesValueToDate()
        {
            if (_spsk != null)
                return _allocation.GetCloseSalesValueToDate(spsk: _spsk);

            return 0.0M;
        }

        public decimal GetSalesValueToYear()
        {
            var fromDate = int.Parse($"{GeneralUtility.ServerYear}0101");
            var toDate = int.Parse($"{GeneralUtility.ServerYear}1231");

            if (_spsk != null)
                return _allocation.GetCloseSalesValueToYear(spsk: _spsk,
                      fromDate: fromDate,
                      toDate: toDate);

            return 0.0M;
        }

        public decimal FindGrossToNextPromote()
        {
            try
            {
                PersonEntity salePersonEntity;
                EmploymentEntity empoymentEntity;
                CommissionStructureDetailEntity schemeDetailEntity;

                salePersonEntity = _person.Finds(filter: f => f.SysKey == _spsk).ToList()[0];

                if (salePersonEntity.TransType != (int)TransType.InternalSalesperson)
                    return decimal.Zero;

                empoymentEntity = _employment.Finds(filter: f => f.SysKey == _spsk).Single();
                schemeDetailEntity = _commScheme.Finds(filter: f => f.SysKey == empoymentEntity.SchemeDetailSysKey).Single();

                var closeSalesToDate = GetSalesCommissionToDate();
                if (!IsLastScheme(schemeDetailEntity.SysKey))
                    return schemeDetailEntity.MaxCom - (closeSalesToDate + empoymentEntity.OpeningAccSales);
                else
                    return decimal.Zero;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities
       
        private bool IsLastScheme(string currentSchemeSK)
        {
            try
            {
                if (currentSchemeSK == _commScheme.FindBySPSK(_spsk).Last().SysKey)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
