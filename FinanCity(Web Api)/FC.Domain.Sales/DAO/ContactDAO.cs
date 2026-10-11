namespace FC.Domain.Sales.DAO
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;

    public class ContactDAO : DAO<ContactEntity>
    {
        public string FullAddress(string key)
        {
            try
            {
                var query = $@"SELECT dbo.AddressFrmByKey('{key}')";
                return (string)DbExcutor.ExcuteScalar(query, CommonConstant.SalesConnection);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
