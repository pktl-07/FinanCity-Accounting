namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
 
    public class CareerManagement : BusinessLogic<CareerManagementEntity, CareerManagementDAO>
    {
        #region Fields

        private Person _person;
        private Employment _employment;
     
        #endregion

        #region Ctor

        public CareerManagement(Person person,
            Employment employment)
        {
            _person = person;
            _employment = employment;
        }

        #endregion

        #region Methods

        public IDataPagerResult<CareerManagementDTO> GetPager(int currentPage, int pageSize, Expression<Func<CareerManagementDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public CareerManagementEntity InsertAsTransaction(CareerManagementEntity entity)
        {
            try
            {
                var emp = _employment.Finds(filter: f => f.SysKey == entity.SPSK).Single();

                entity = Insert(entity: entity);

                emp.SchemeDetailSysKey = entity.SchemeDetailSK;
                emp.Rank = entity.Rank;
                emp.L1 = entity.T1;

                _employment.CareerManagementEntity = entity;
                _employment.Update(emp, filter: f => f.SysKey == entity.SPSK);

                if (entity.TransType == (int)TransType.ChangeOfStatus)
                {
                    var person = _person.Finds(filter: f => f.SysKey == entity.SPSK).Single();

                    person.IsActive = entity.IsActive;
                    person.ReasonToTerminate = entity.ReasonToTerminate;

                    _person.Update(person,
                        filter: f => f.SysKey == entity.SPSK);
                }
            
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<SalesPersonTireDTO> GetCareerManagementDownTree(string key)
        {
            try
            {
                return DAO.GetCareerManagementDownTree(key: key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalesPersonTireDTO> GetCareerManagementDownTree(int date)
        {
            try
            {
                return DAO.GetCareerManagementDownTree(date: date);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
