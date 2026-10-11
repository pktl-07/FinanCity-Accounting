namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class Notification
        : BusinessLogic<NotificationEntity, NotificationDAO>
    {
        #region Methods

        public IEnumerable<NotificatonMortageSalesDTO> GetNotificationMortgageSales(
          Expression<Func<NotificatonMortageSalesDTO, bool>> filter)
        {
            try
            {
                return DAO.GetNotificationMortgageSales(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<NotificatonMortageSalesDTO> GetMortgagePager(int currentPage,
            int pageSize,
            Expression<Func<NotificatonMortageSalesDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetMortgagePager(currentPage: currentPage,
                    pageSize: pageSize,
                    filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadFollowUpDTO> GetLeadFollowUpPager(
            int currentPage,
            int pageSize,
            Expression<Func<LeadFollowUpDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetLeadFollowUpPager(currentPage: currentPage,
                  pageSize: pageSize,
                  filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IDataPagerResult<NotificationEntity> GetPager(
            int currentPage,
            int pageSize,
            Expression<Func<NotificationEntity, bool>> filter = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage,
                    pageSize: pageSize,
                    filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<LeadFollowUpDTO> GetLeadFollowUpDTO(
         Expression<Func<LeadFollowUpDTO, bool>> filter)
        {
            try
            {
                return DAO.GetLeadFollowUpDTO(filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ForceNotify(string spsk, TransType transType)
        {
            try
            {
                DAO.ForceNotify(spsk: spsk, transType: transType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
