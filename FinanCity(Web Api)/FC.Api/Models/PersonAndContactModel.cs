namespace FC.Api.Models
{
    using FC.Domain.Sales.Entity;

    public class PersonAndContactModel
    {
        public PersonEntity Person { get; set; }
        public ContactEntity Contact { get; set; }
    }
}