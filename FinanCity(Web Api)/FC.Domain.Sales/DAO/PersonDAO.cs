namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Core.Infrastructure.Interfaces;

    public class PersonDAO : DAO<PersonEntity>
    {
        #region Methods

        public IDataPagerResult<PersonEntity> GetPager(int currentPage, int pageSize, Expression<Func<PersonEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<PersonEntity>()} WHERE {where.ToSQL()}
                ORDER BY {With<PersonEntity>(x => x.Name)}";

                return ExcutePagerResult<PersonEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetPersonalContactPager(int currentPage, int pageSize, Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT 
                {With<PersonEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.SysKey)}],
                {With<PersonEntity>(x => x.R1)} [{nameof(LeadAndContactDTO.R1)}],
                {With<PersonEntity>(x => x.Name)} [{nameof(LeadAndContactDTO.Name)}],
                {With<PersonEntity>(x => x.NRIC)} [{nameof(LeadAndContactDTO.NRIC)}],
                {With<PersonEntity>(x => x.Designation)} [{nameof(LeadAndContactDTO.Designation)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(LeadAndContactDTO.Address)}],
                {With<ContactEntity>(x => x.MobilePhone)} [{nameof(LeadAndContactDTO.Mobile)}],
                {With<ContactEntity>(x => x.Email)} [{nameof(LeadAndContactDTO.Email)}],
                {With<ContactEntity>(x => x.Fax)} [{nameof(LeadAndContactDTO.Fax)}],
                {With<PersonEntity>(x => x.IsActive)} [{nameof(LeadAndContactDTO.IsActive)}],
                {With<PersonEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.TransType)}],
                {With<PersonEntity>(x => x.IsOrganization)} [{nameof(LeadAndContactDTO.IsOrganization)}],
                {With<CompanyEntity>(x => x.CompanyName)} [{nameof(LeadAndContactDTO.CompanyName)}],
                {With<CompanyEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.VendorType)}],
                NULL [{nameof(LeadAndContactDTO.RefSK)}],
                {With<CompanyEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.VendorSK)}]
                FROM {Table<PersonEntity>()}
                LEFT JOIN {Table<ContactEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()}
                    ORDER BY [tbl].SysKey DESC";

                return ExcutePagerResult<LeadAndContactDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetClientPager(int currentPage, int pageSize, Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT 
                {With<PersonEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.SysKey)}],
                {With<PersonEntity>(x => x.R1)} [{nameof(LeadAndContactDTO.R1)}],
                {With<PersonEntity>(x => x.Name)} [{nameof(LeadAndContactDTO.Name)}],
                {With<PersonEntity>(x => x.NRIC)} [{nameof(LeadAndContactDTO.NRIC)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(LeadAndContactDTO.Address)}],
                {With<ContactEntity>(x => x.Email)} [{nameof(LeadAndContactDTO.Email)}],
                {With<ContactEntity>(x => x.Fax)} [{nameof(LeadAndContactDTO.Fax)}],
                {With<PersonEntity>(x => x.IsActive)} [{nameof(LeadAndContactDTO.IsActive)}],
                {With<PersonEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.TransType)}],
                {With<PersonEntity>(x => x.IsOrganization)} [{nameof(LeadAndContactDTO.IsOrganization)}],
                {With<CompanyEntity>(x => x.CompanyName)} [{nameof(LeadAndContactDTO.CompanyName)}],
                {With<CompanyEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.VendorType)}],
                {With<CompanyEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.VendorSK)}],
                {With<ClientShareEntity>(x => x.RefSK)} [{nameof(LeadAndContactDTO.RefSK)}]
                FROM {Table<PersonEntity>()}
                INNER JOIN {Table<ClientShareEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ClientShareEntity>(x => x.SysKey)}
                LEFT JOIN {Table<ContactEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}";

                query = $@"SELECT * FROM ({query}) [tbl] 
                    WHERE {where.ToSQL()}
                    ORDER BY [tbl].SysKey DESC";

                return ExcutePagerResult<LeadAndContactDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<PersonEntity> GetClientAndLead(Expression<Func<PersonEntity, bool>> where = null)
        {
            try
            {
                where = where.Constant(x => x.TransType == (int)TransType.Client ||
                x.TransType == (int)TransType.Lead);

                var query = $@"
                    SELECT {Table<PersonEntity>()}.* 
                    FROM {Table<PersonEntity>()}
                    INNER JOIN {Table<ClientShareEntity>()} ON {With<ClientShareEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.SysKey)}
                    WHERE {where.ToSQL()} 
                    AND {With<ClientShareEntity>(x => x.RefSK)} = '{RefSK}'
                    UNION
                    SELECT {Table<PersonEntity>()}.* 
                    FROM {Table<PersonEntity>()}
                    INNER JOIN {Table<ClientShareEntity>()} ON {With<ClientShareEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.SysKey)}
                    LEFT JOIN {Table<LeadEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<LeadEntity>(x => x.SysKey)}
                    WHERE {With<ClientShareEntity>(x => x.RefSK)} = '{RefSK}'
                    AND {where.ToSQL()}";

                return DbExcutor<PersonEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<EmploymentDTO> GetEmploymentPager(int currentPage, int pageSize, Expression<Func<EmploymentDTO, bool>> where = null)
        {
            try
            {
                var r = DI.Current.Get<IEssDataRepository>();
                var accountdb = "[" + new SqlConnectionStringBuilder(r.DbConnection[CommonConstant.AccountConnection].ConnectionString).InitialCatalog + "]";

                var query = $@"SELECT 
                {With<PersonEntity>(x => x.SysKey)} [{nameof(EmploymentDTO.SysKey)}],
                {With<PersonEntity>(x => x.R1)} [{nameof(EmploymentDTO.R1)}],
                {With<PersonEntity>(x => x.Name)} [{nameof(EmploymentDTO.Name)}],
                {With<PersonEntity>(x => x.NRIC)} [{nameof(EmploymentDTO.NRIC)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)})  [{nameof(EmploymentDTO.Address)}],
                {With<PersonEntity>(x => x.CEANumber)} [{nameof(EmploymentDTO.CEANumber)}],
                {With<DepartmentEntity>(x => x.Description)} [{nameof(EmploymentDTO.Department)}],
                NULL [{nameof(EmploymentDTO.ManagerName)}],
                NULL [{nameof(EmploymentDTO.DirectorName)}],
                {With<CommissionStructureDetailEntity>(x => x.Designation)} [{nameof(EmploymentDTO.Designation)}],
                (SELECT {With<CompanyEntity>(x => x.CompanyName)} FROM {Table<CompanyEntity>()} WHERE {With<CompanyEntity>(x => x.TransType)}={(int)TransType.Franchisee} AND {With<CompanyEntity>(x => x.SysKey)}={With<PersonEntity>(x => x.FranchiseeSK)})  [{nameof(EmploymentDTO.Franchisee)}],
                (SELECT {With<CommissionStructureEntity>(x => x.SchemeName)} FROM {Table<CommissionStructureEntity>()} WHERE {With<CommissionStructureEntity>(x => x.SysKey)} = {With<CommissionStructureDetailEntity>(x => x.ParentSK)}) [{nameof(EmploymentDTO.CommissionScheme)}],               
                {With<PersonEntity>(x => x.IsActive)} [{nameof(EmploymentDTO.IsActive)}],
                {With<PersonEntity>(x => x.TransType)} [{nameof(EmploymentDTO.TransType)}],
                {With<PersonEntity>(x => x.VendorSK)} [{nameof(EmploymentDTO.VendorSK)}],
                {With<ContactEntity>(x => x.MobilePhone)} [{nameof(EmploymentDTO.MobilePhone)}],
                {With<PersonEntity>(x => x.ReasonToTerminate)} [{nameof(EmploymentDTO.Status)}]
                FROM 
                {Table<PersonEntity>()}
                INNER JOIN {Table<ContactEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<ContactEntity>(x => x.SysKey)}
                INNER JOIN {Table<EmploymentEntity>()} ON {With<EmploymentEntity>(x => x.SysKey)}={With<PersonEntity>(x => x.SysKey)}
                INNER JOIN {Table<CommissionStructureDetailEntity>()} ON {With<CommissionStructureDetailEntity>(x => x.SysKey)}={With<EmploymentEntity>(x => x.SchemeDetailSysKey)}
                LEFT JOIN {accountdb}..[Department] ON {With<PersonEntity>(x => x.DepartmentSK)}={With<DepartmentEntity>(x => x.SysKey)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()}
                    ORDER BY [tbl].SysKey DESC";

                return ExcutePagerResult<EmploymentDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalesPersonTireDTO> GetSalepersonTire()
        {
            try
            {
                var query = $@" SELECT 
                     ROW_NUMBER() OVER(ORDER BY {nameof(SalesPersonTireDTO.GroupNum)}) [{nameof(SalesPersonTireDTO.RowNumber)}], 
                     SPSK [{nameof(SalesPersonTireDTO.SPSK)}],
                     SPName [{nameof(SalesPersonTireDTO.Name)}],
                     ParentSK [{nameof(SalesPersonTireDTO.ParentSK)}],
                     Designation [{nameof(SalesPersonTireDTO.Designation)}],
                     Level [{nameof(SalesPersonTireDTO.Level)}],
                     Rank [{nameof(SalesPersonTireDTO.Rank)}],
                     GroupNum [{nameof(SalesPersonTireDTO.GroupNum)}],
                     IsActive [{nameof(SalesPersonTireDTO.IsActive)}]
                     FROM [dbo].[GetSalepersonTire]()";

                return DbExcutor<SalesPersonTireDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalesPersonTireDTO> GetCareerManagementDownTree(int? date)
        {
            try
            {
                var query = $@" SELECT 
                     ROW_NUMBER() OVER(ORDER BY {nameof(SalesPersonTireDTO.GroupNum)}) [{nameof(SalesPersonTireDTO.RowNumber)}], 
                     SPSK [{nameof(SalesPersonTireDTO.SPSK)}],
                     Name [{nameof(SalesPersonTireDTO.Name)}],
                     T1 [{nameof(SalesPersonTireDTO.ParentSK)}],
                     Designation [{nameof(SalesPersonTireDTO.Designation)}],
                     Level [{nameof(SalesPersonTireDTO.Level)}],
                     Rank [{nameof(SalesPersonTireDTO.Rank)}],
                     Payout [{nameof(SalesPersonTireDTO.Payout)}],
                     GroupNum [{nameof(SalesPersonTireDTO.GroupNum)}],
                     NRIC [{nameof(SalesPersonTireDTO.NRIC)}],
                     IsActive [{nameof(SalesPersonTireDTO.IsActive)}]
                     FROM [dbo].[GetCareerManagementTierByDate]({date})";

                return DbExcutor<SalesPersonTireDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<IndustryExamEntity> GetIndustryExamByKey(string spsk)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<IndustryExamEntity>()} 
                WHERE {With<IndustryExamEntity>(x => x.SysKey)} IN 
                (SELECT {With<PersonIndustryEntity>(x => x.IndusSysKey)} FROM {Table<PersonIndustryEntity>()} WHERE {With<PersonIndustryEntity>(x => x.SPSysKey)}='{spsk}')";

                return DbExcutor<IndustryExamEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<QualificationEntity> GetQualificationExamByKey(string spsk)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<QualificationEntity>()} 
                WHERE {With<QualificationEntity>(x => x.SysKey)} IN 
                (SELECT {With<PersonEducationEntity>(x => x.EduSysKey)} FROM {Table<PersonEducationEntity>()} WHERE {With<PersonEducationEntity>(x => x.SPSysKey)}='{spsk}')";

                return DbExcutor<QualificationEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Members

        public string RefSK
        {
            get
            {
                return DI.Current.Get<IFinanCityIdentity>().SPSK;
            }
        }

        #endregion
    }
}
