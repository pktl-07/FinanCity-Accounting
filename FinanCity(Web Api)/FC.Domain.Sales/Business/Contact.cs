namespace FC.Domain.Sales.Business
{
    using System;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class Contact : BusinessLogic<ContactEntity, ContactDAO>
    {
        public string FullAddress(string key)
        {
            try
            {
                return DAO.FullAddress(key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
