namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;

    public class PersonAndContactModel
    {
        public PersonEntity Person { get; set; }
        public ContactEntity Contact { get; set; }
    }
}