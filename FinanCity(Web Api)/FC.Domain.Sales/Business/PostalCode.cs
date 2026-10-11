namespace FC.Domain.Sales.Business
{
    using System;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class PostalCode : BusinessLogic<PostalCodeEntity, PostalCodeDAO>
    {
        #region Methods

        public FullAddressDTO FindFullAddress(string postalCode)
        {
            try
            {
                return DAO.FindFullAddress(postalCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
