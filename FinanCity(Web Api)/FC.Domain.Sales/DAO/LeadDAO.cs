namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    public class LeadDAO : DAO<LeadEntity>
    {
        #region Methods

        public IDataPagerResult<LeadManagementDTO> GetPager(int currentPage, int pageSize, Expression<Func<LeadManagementDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT {With<PersonEntity>(x => x.SysKey)} [{nameof(LeadManagementDTO.SysKey)}],
                    {With<LeadEntity>(x => x.R1)} [{nameof(LeadManagementDTO.LeadNo)}],
                    {With<PersonEntity>(x => x.Name)} [{nameof(LeadManagementDTO.Name)}],
                    {With<PersonEntity>(x => x.IsOrganization)} [{nameof(LeadManagementDTO.IsOrganization)}],
                    {With<PersonEntity>(x => x.NRIC)} [{nameof(LeadManagementDTO.NRIC)}],
                    {With<FollowUpEntity>(x => x.StartDate)} [{nameof(LeadManagementDTO.FollowupDate)}],
                    dbo.AddressFrmByKey({With<PersonEntity>(x => x.SysKey)}) [{nameof(LeadManagementDTO.Address)}],
                    {With<TNC_PropertyTypeEntity>(x => x.Description)} [{nameof(LeadManagementDTO.PropertyType)}],
                    {With<ContactEntity>(x => x.MobilePhone)} [{nameof(LeadManagementDTO.Mobile)}],
                    {With<ContactEntity>(x => x.Email)} [{nameof(LeadManagementDTO.Email)}],
                    {With<LeadEntity>(x => x.SalesActivity)} [{nameof(LeadManagementDTO.SalesActivity)}],
                    {With<LeadEntity>(x => x.LeadCloseRate)} [{nameof(LeadManagementDTO.LeadCloseRate)}],
                    {With<PersonEntity>(x => x.IsActive)} [{nameof(LeadManagementDTO.IsActive)}],
                    {With<ClientShareEntity>(x => x.RefSK)} [{nameof(LeadManagementDTO.RefSK)}],
                    (SELECT [t2].Name FROM {Table<PersonEntity>()} [t2] WHERE [t2].SysKey = (
	                SELECT TOP 1 [t1].RefSK FROM  {Table<ClientShareEntity>()} [t1]
                    WHERE [t1].[SysKey] = {With<PersonEntity>(x => x.SysKey)} AND [t1].RefSK <> '1')) [{nameof(LeadManagementDTO.SalespersonName)}]
                    FROM {Table<LeadEntity>()}
                    INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<LeadEntity>(x => x.SysKey)}
                    INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<LeadEntity>(x => x.SysKey)}
                    INNER JOIN {Table<FollowUpEntity>()} ON {With<FollowUpEntity>(x => x.RefSK)} = {With<LeadEntity>(x => x.SysKey)}
                    INNER JOIN {Table<ClientShareEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ClientShareEntity>(x => x.SysKey)}
                    LEFT JOIN {Table<TNC_PropertyTypeEntity>()} ON {With<TNC_PropertyTypeEntity>(x => x.SysKey)} = {With<LeadEntity>(x => x.PropertyTypeSK)}
                    WHERE {With<PersonEntity>(x => x.TransType)} = {(int)TransType.Lead}";

                query = $@"SELECT * FROM ({query}) [tbl]
                WHERE {where.ToSQL()}
                ORDER BY [{nameof(LeadManagementDTO.SysKey)}] DESC";

                return ExcutePagerResult<LeadManagementDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<string> GetLeadWhoNeverBecomeClientFor(int day)
        {
            try
            {
                var query = $@"SELECT SysKey FROM (
	                SELECT {With<PersonEntity>(x => x.SysKey)} [{nameof(PersonEntity.SysKey)}],
                    {With<PersonEntity>(x => x.Name)} [{nameof(PersonEntity.Name)}],
                    {With<PersonEntity>(x => x.NRIC)} [{nameof(PersonEntity.NRIC)}],
	                DATEDIFF(DAY, dbo.AddCustomDayFormat({With<PersonEntity>(x => x.CreatedDate)}, 0) , CAST(dbo.GETSEVERDATE() AS VARCHAR(50))) [Day]
	                FROM {Table<PersonEntity>()} 
                    WHERE {With<PersonEntity>(x => x.TransType)} = {(int)TransType.Lead}) [tbl]
                    WHERE Day >= {day}";

                return DbExcutor<PersonEntity>.ExcuteReader(query).Select(x => x.SysKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<LeadEntity> GetLead(Expression<Func<LeadEntity, bool>> filter)
        {
            try
            {
                var identity = DI.Current.Get<IFinanCityIdentity>();
                var query = $@"SELECT { Table<LeadEntity>()}.*
                            FROM {Table<LeadEntity>()}
                            INNER JOIN { Table<ClientShareEntity>()}
                            ON { With<LeadEntity>(x => x.SysKey)} = { With<ClientShareEntity>(x => x.SysKey)}
                            WHERE { filter.ToSQL()}
                            AND {With<ClientShareEntity>(x => x.RefSK)} = '{identity.SPSK}'";

                return DbExcutor<LeadEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
