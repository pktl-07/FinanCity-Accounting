namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    public class SelespersonWhoHasResetScheme : ICommissionBaseHelperFactory
    {
        #region Fields

        private string _effectiveDate;
        private string _spsk;
        private Person _person;
        private Employment _employment;
        private CommissionStructureDetail _schemeDetail;
        private CommissionAllocation _allocation;

        #endregion

        #region Ctor

        //this parameterless constructor is required for Ninject
        public SelespersonWhoHasResetScheme() { }

        public SelespersonWhoHasResetScheme(Person person, 
            Employment employment,
            CommissionAllocation allocation,
            CommissionStructureDetail schemeDetail,
            string spsk,
            string effectiveDate)
        {
            _allocation = allocation;
            _person = person;
            _employment = employment;
            _schemeDetail = schemeDetail;
            _spsk = spsk;
            _effectiveDate = effectiveDate;
        }

        #endregion

        #region Methods

        public decimal FindGrossToNextPromote()
        {
            try
            {
                PersonEntity salePersonEntity;
                EmploymentEntity empoymentEntity;
                CommissionStructureDetailEntity schemeDetailEntity;

                salePersonEntity = _person.Finds(filter: f => f.SysKey == _spsk).Single();

                if (salePersonEntity.TransType != (int)TransType.InternalSalesperson)
                    return decimal.Zero;

                empoymentEntity = _employment.Finds(filter: f => f.SysKey == _spsk).Single();
                schemeDetailEntity = _schemeDetail.Finds(filter: f => f.SysKey == empoymentEntity.SchemeDetailSysKey).Single();

                if (!IsLastScheme(schemeDetailEntity.SysKey))
                    return schemeDetailEntity.MaxCom - (GetSalesCommissionToYear() + empoymentEntity.OpeningAccSales);
                else
                    return decimal.Zero;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetSalesCommissionToDate()
        {
            try
            {
                return _allocation.GetSalesCommissionToDate(_spsk);
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
                var fromDate = int.Parse($"{GeneralUtility.ServerYear}0101");
                var toDate = int.Parse($"{GeneralUtility.ServerYear}1231");

                return _allocation.GetSalesCommissionToYear(spsk: _spsk,
                    fromDate: fromDate,
                    toDate: toDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetSalesValueToDate()
        {
            try
            {
                return _allocation.GetCloseSalesValueToDate(spsk: _spsk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetSalesValueToYear()
        {
            try
            {
                var fromDate = int.Parse($"{GeneralUtility.ServerYear}0101");
                var toDate = int.Parse($"{GeneralUtility.ServerYear}1231");

                return _allocation.GetCloseSalesValueToYear(spsk: _spsk,
                    fromDate: fromDate,
                    toDate: toDate);
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
                if (currentSchemeSK == _schemeDetail.FindBySPSK(_spsk).Last().SysKey)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetAccurateRealStartDate()
        {
            var end = GeneralUtility.ConvertDateFormat(GeneralUtility.ServerDate.ToString()).Value;
            var start = GeneralUtility.ConvertDateFormat(GeneralUtility.ConvertSystemDateStringFormat(_effectiveDate)).Value;

            if (_effectiveDate == GeneralUtility.ServerDate.ToString())
            {
                return int.Parse(_effectiveDate);
            }

            var yearDifferent = (end.Year - start.Year - 1) +
                (((end.Month > start.Month) ||
                ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0);

            start = start.AddYears(yearDifferent);
            return int.Parse($"{start.Year}{start.Month.ToString("00")}{start.Day.ToString("00")}");
        }

        #endregion
    }
}
