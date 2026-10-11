namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class CareerManagementDAO : DAO<CareerManagementEntity>
    {
        #region Methods

        public IDataPagerResult<CareerManagementDTO> GetPager(int currentPage, int pageSize, Expression<Func<CareerManagementDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM ({CareerManagementQuery}) [tbl]
                    WHERE {where.ToSQL()}
                    ORDER BY [tbl].CreateDate DESC";

                return ExcutePagerResult<CareerManagementDTO>(query, currentPage, pageSize);
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
                var query = $@" SELECT 
                     ROW_NUMBER() OVER(ORDER BY {nameof(SalesPersonTireDTO.GroupNum)}) [{nameof(SalesPersonTireDTO.RowNumber)}], 
                     SPSK [{nameof(SalesPersonTireDTO.SPSK)}],
                     Name [{nameof(SalesPersonTireDTO.Name)}],
                     T1 [{nameof(SalesPersonTireDTO.ParentSK)}],
                     Designation [{nameof(SalesPersonTireDTO.Designation)}],
                     Level [{nameof(SalesPersonTireDTO.Level)}],
                     Rank [{nameof(SalesPersonTireDTO.Rank)}],
                     GroupNum [{nameof(SalesPersonTireDTO.GroupNum)}],
                     NRIC [{nameof(SalesPersonTireDTO.NRIC)}],
                     IsActive [{nameof(SalesPersonTireDTO.IsActive)}],
                     Payout [{nameof(SalesPersonTireDTO.Payout)}]
                     FROM [dbo].[GetCareerManagementTierByKey]('{key}')";

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

        #endregion

        #region Utilities

        private string CareerManagementQuery
        {
            get
            {
                var query = $@"SELECT 
                  {With<CareerManagementEntity>(x => x.SysKey)} [{nameof(CareerManagementDTO.SysKey)}],
                  {With<CareerManagementEntity>(x => x.CreateDate)} [{nameof(CareerManagementDTO.CreateDate)}],
                  {With<LoggerEntity>(x => x.OldValue)} [{nameof(CareerManagementDTO.From)}],
                  {With<LoggerEntity>(x => x.NewValue)} [{nameof(CareerManagementDTO.To)}],
                  {With<CareerManagementEntity>(x => x.EffectiveDate)} [{nameof(CareerManagementDTO.EffectiveDate)}],
                  {With<CareerManagementEntity>(x => x.ApprovedBy)} [{nameof(CareerManagementDTO.ApprovedBy)}],
                  {With<CareerManagementEntity>(x => x.Remarks)} [{nameof(CareerManagementDTO.Remarks)}],
                  {With<PersonEntity>(x => x.Name)} [{nameof(CareerManagementDTO.SalepersonName)}],
                  {With<PersonEntity>(x => x.NRIC)} [{nameof(CareerManagementDTO.NRIC)}],
                  {With<PersonEntity>(x => x.CEANumber)} [{nameof(CareerManagementDTO.CEANumber)}],
                  {With<CareerManagementEntity>(x => x.BonusPercent)} [{nameof(CareerManagementDTO.Payout)}],
                  {With<CareerManagementEntity>(x => x.IsDefault)} [{nameof(CareerManagementDTO.IsDefault)}],
                  {With<CareerManagementEntity>(x => x.TransType)} [{nameof(CareerManagementDTO.TransType)}],
                  CASE 
                    WHEN {With<CareerManagementEntity>(x => x.TransType)} = {(int)TransType.BranchOut} THEN '{TransType.BranchOut.ToDescription()}' 
                    WHEN {With<CareerManagementEntity>(x => x.TransType)} = {(int)TransType.ChangeOfStatus} THEN '{TransType.ChangeOfStatus.ToDescription()}'
                  END [{nameof(CareerManagementDTO.Action)}]
                  FROM  {Table<PersonEntity>()}
                  INNER JOIN {Table<CareerManagementEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<CareerManagementEntity>(x => x.SPSK)}
                  LEFT JOIN {Table<LoggerEntity>()} ON {With<LoggerEntity>(x => x.CMSK)} = {With<CareerManagementEntity>(x => x.SysKey)}";
                    
                return query;
            }
        }

        #endregion
    }
}
