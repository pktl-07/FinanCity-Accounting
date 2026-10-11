namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;

    public class SalesInvoiceModel
    {
        public SalesInvoiceEntity SalesInvoice { get; set; }
        public ContactEntity Contact { get; set; }
    }
}