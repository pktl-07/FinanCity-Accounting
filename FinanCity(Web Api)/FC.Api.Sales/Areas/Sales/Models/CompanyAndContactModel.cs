namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;

    public class CompanyAndContactModel
    {
        public CompanyEntity Vendor { get; set; }
        public ContactEntity Contact { get; set; }
    }
}