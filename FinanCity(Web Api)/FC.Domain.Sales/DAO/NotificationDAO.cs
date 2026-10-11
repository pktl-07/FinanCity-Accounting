namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
   
    public class NotificationDAO : DAO<NotificationEntity>
    {
        public IEnumerable<NotificatonMortageSalesDTO> GetNotificationMortgageSales(
            Expression<Func<NotificatonMortageSalesDTO, bool>> filter)
        {
            try
            {
                var query = MortgageQuery;
                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}";

                return DbExcutor<NotificatonMortageSalesDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<LeadFollowUpDTO> GetLeadFollowUpDTO(
            Expression<Func<LeadFollowUpDTO,bool>> filter)
        {
            try
            {
                var query = $@"{FollowUpQuery} AND {filter.ToSQL()}";

                return DbExcutor<LeadFollowUpDTO>.ExcuteReader(query);
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
                var query = $@"{FollowUpQuery} AND {filter.ToSQL()}
                ORDER BY FollowUpSK DESC";

                return ExcutePagerResult<LeadFollowUpDTO>(query,
                    currentPage,
                    pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<NotificatonMortageSalesDTO> GetMortgagePager(
            int currentPage,
            int pageSize,
            Expression<Func<NotificatonMortageSalesDTO, bool>> filter = null)
        {
            try
            {
                var query = MortgageQuery;
                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()} ORDER BY SysKey";

                return ExcutePagerResult<NotificatonMortageSalesDTO>(query,
                    currentPage,
                    pageSize);
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
                var query = $@"SELECT * FROM {Table<NotificationEntity>()}
                    WHERE {filter.ToSQL()}";

                query = $@"SELECT * FROM ({query})[tbl]
                ORDER BY [SysKey]";

                return ExcutePagerResult<NotificationEntity>(query,
                 currentPage,
                 pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ForceNotify(string spsk,TransType transType)
        {
            try
            {
                var query = $@"SELECT TOP 1 * FROM {Table<NotificationEntity>()}
                    WHERE {With<NotificationEntity>(x => x.ReceiverSK)} = <# {spsk} #>
                    AND {With<NotificationEntity>(x => x.TransType)} = <# {(int)transType} #>";

                var entity = DbExcutor<NotificationEntity>.ExcuteReader(query).FirstOrDefault();              
                if (entity != null)
                {
                    DbExcutor<NotificationEntity>.ExcuteNonQuery($@"
                        UPDATE {Table<NotificationEntity>()}
                        SET {With<NotificationEntity>(x => x.ReceiverSK)} = <# {entity.ReceiverSK} #> 
                        WHERE {With<NotificationEntity>(x => x.SysKey)} = <# {entity.SysKey} #>
                    ");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string MortgageQuery
        {
            get
            {
                return $@"SELECT 
                {With<NotificationEntity>(x => x.SysKey)} [{nameof(NotificatonMortageSalesDTO.SysKey)}],
                {With<NotificationEntity>(x => x.CreatedDate)} [{nameof(NotificatonMortageSalesDTO.CreatedDate)}],
                {With<NotificationEntity>(x => x.ReceiverSK)} [{nameof(NotificatonMortageSalesDTO.ReceiverSK)}],
                {With<NotificationEntity>(x => x.SenderSK)} [{nameof(NotificatonMortageSalesDTO.SenderSK)}],
                {With<NotificationEntity>(x => x.Description)} [{nameof(NotificatonMortageSalesDTO.Description)}],
                {With<PropertyTransactionEntity>(x => x.MailingClientSK)} [{nameof(NotificatonMortageSalesDTO.MaillingClientSK)}],
                {With<PropertyTransactionEntity>(x => x.TransType)} [{nameof(NotificatonMortageSalesDTO.TransType)}],
                {With<NotificationEntity>(x => x.RefSK)} [{nameof(NotificatonMortageSalesDTO.RefSK)}],
                {With<NotificationEntity>(x => x.IsRead)} [{nameof(NotificatonMortageSalesDTO.IsRead)}]
                FROM {Table<NotificationEntity>()} 
                INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<NotificationEntity>(x => x.RefSK)}";
            }
        }

        private string FollowUpQuery
        {
            get
            {
                var query = $@"SELECT * FROM (SELECT
                {With<NotificationEntity>(x => x.SysKey)} [NotificationSK],    
                {With<FollowUpEntity>(x => x.SysKey)} [FollowUpSK],
                {With<FollowUpEntity>(x => x.RefSK)} [LeadSK],
                {With<NotificationEntity>(x => x.IsRead)} [IsRead],    
                {With<PersonEntity>(x => x.Name)} [LeadName],
                {With<FollowUpEntity>(x => x.StartDate)} [StartDate],
                {With<FollowUpEntity>(x => x.StartTime)} [StartTime],
                {With<FollowUpEntity>(x => x.Description)} [Description],
                {With<NotificationEntity>(x => x.SenderSK)} [SenderSK],
                {With<NotificationEntity>(x => x.ReceiverSK)} [ReceiverSK],
                dbo.AddCustomDayFormat({With<FollowUpEntity>(x => x.StartDate)},{With<FollowUpEntity>(x => x.NotifyMeBeforeStartDate)}) [NotifyDate]
                FROM 
                {Table<FollowUpEntity>()}
                INNER JOIN {Table<NotificationEntity>()} ON {With<FollowUpEntity>(x => x.SysKey)} = {With<NotificationEntity>(x => x.RefSK)}
                INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<FollowUpEntity>(x => x.RefSK)}
                WHERE {With<FollowUpEntity>(x => x.Notify)} = 1
                AND {With<NotificationEntity>(x => x.TransType)} = {(int)TransType.LeadFollowUp}) [tbl]
                WHERE [tbl].[NotifyDate] <= dbo.GETSEVERDATE()";

                return query;
            }
        }
        
    }
}
