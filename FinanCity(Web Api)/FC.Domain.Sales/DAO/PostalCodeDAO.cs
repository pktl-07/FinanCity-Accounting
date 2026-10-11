namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq;
    using FC.Core.Data;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class PostalCodeDAO : DAO<PostalCodeEntity>
    {
        #region Methods

        public FullAddressDTO FindFullAddress(string postalCode)
        {

            var query = $@"SELECT 
                 {With<PostalCodeEntity>(x => x.BuildingNumber)} [{nameof(FullAddressDTO.Blk)}],
                 {With<StreetEntity>(x => x.StreetName)} [{nameof(FullAddressDTO.Address1)}],
                 {With<StreetEntity>(x => x.SysKey)} [{nameof(FullAddressDTO.StreetSK)}],
                 {With<BuildingEntity>(x => x.SysKey)} [{nameof(FullAddressDTO.BuildingSK)}],
                 {With<BuildingEntity>(x => x.TypeFlag)} [{nameof(FullAddressDTO.TypeFlag)}],
                 {With<PostalCodeEntity>(x => x.AddressType)} [{nameof(FullAddressDTO.AddressType)}],
                 CASE 
                 WHEN {With<BuildingEntity>(x => x.TypeFlag)}= '0'
                 THEN  RTRIM(LTRIM({With<BuildingEntity>(x => x.BuildingName)}))
                 ELSE
                 RTRIM(LTRIM({With<BuildingEntity>(x => x.TypeFlag)})) + ' ' + RTRIM(LTRIM({With<BuildingEntity>(x => x.BuildingName)}))
                 END [Address2],
                 {With<PostalCodeEntity>(x => x.SysKey)} [{nameof(FullAddressDTO.PostalCode)}]
                 FROM {Table<PostalCodeEntity>()}
                 INNER JOIN {Table<StreetEntity>()} ON {With<StreetEntity>(x => x.SysKey)}={With<PostalCodeEntity>(x => x.StreetSK)}
                 LEFT JOIN {Table<BuildingEntity>()} ON {With<BuildingEntity>(x => x.SysKey)}={With<PostalCodeEntity>(x => x.BuildingSK)}
                 WHERE {With<PostalCodeEntity>(x => x.SysKey)}= '{postalCode}'";
            try
            {
                return DbExcutor<FullAddressDTO>.ExcuteReader(query).SingleOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
