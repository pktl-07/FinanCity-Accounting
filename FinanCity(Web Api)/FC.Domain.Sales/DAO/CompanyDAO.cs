namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
   
    public class CompanyDAO : DAO<CompanyEntity>
    {
        #region Methods

        public IDataPagerResult<CompanyAndContactDTO> GetPager(int currentPage, int pageSize, Expression<Func<CompanyAndContactDTO, bool>> where = null)
        {
            var query = $@"SELECT 
                {With<CompanyEntity>(x => x.SysKey)} [{nameof(CompanyAndContactDTO.SysKey)}],
                {With<CompanyEntity>(x => x.R1)} [{nameof(CompanyAndContactDTO.VendorNo)}],
                {With<CompanyEntity>(x => x.TransType)} [{nameof(CompanyAndContactDTO.VendorType)}],
                {With<CompanyEntity>(x => x.CompanyName)} [{nameof(CompanyAndContactDTO.CompanyName)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(CompanyAndContactDTO.Address)}],
                {With<CompanyEntity>(x => x.CompanyReg)} [{nameof(CompanyAndContactDTO.ACRANo)}],
                {With<CompanyEntity>(x => x.IsActive)} [{nameof(CompanyAndContactDTO.IsActive)}],
                {With<CompanyEntity>(x => x.ContactPerson)} [{nameof(CompanyAndContactDTO.ContactPerson)}],
                {With<CompanyEntity>(x => x.TransType)} [{nameof(CompanyAndContactDTO.TransType)}]
                FROM {Table<CompanyEntity>()} 
                INNER JOIN {Table<ContactEntity>()} ON {With<CompanyEntity>(x => x.SysKey)}={With<ContactEntity>(x => x.SysKey)}";

            query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()} ORDER BY [tbl].CompanyName";

            try
            {
                return ExcutePagerResult<CompanyAndContactDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
