namespace FC.Domain.Sales.Extensions
{
    using System;
    using FC.Domain.Sales.Entity;
    using Newtonsoft.Json;

    public static class EntityExtensions
    {
        public static SalesInvoiceEntity ConvertInvoice(this SalesInvoiceLogEntity entity)
        {
            try
            {
                var json = JsonConvert.SerializeObject(entity);
                return JsonConvert.DeserializeObject<SalesInvoiceEntity>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
