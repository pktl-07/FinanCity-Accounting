namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using System.Collections.Generic;
    using FC.Domain.Sales.Entity;

    public class LeadModel
    {
        public LeadEntity Lead { get; set; }

        public PersonEntity Person { get; set; }

        public ContactEntity Contact { get; set; }

        public FollowUpEntity FollowUp { get; set; }

        public IEnumerable<ChattingEntity> Remarks { get; set; }

        public IEnumerable<string> ShareContacts { get; set; }
    }

    public class LeadCreateModal
    {
        public LeadEntity Lead { get; set; }

        public PersonEntity Person { get; set; }

        public ContactEntity Contact { get; set; }

        public FollowUpEntity FollowUp { get; set; }
    }

    public class LeadUpdateModal : LeadCreateModal
    {
        public IEnumerable<ChattingEntity> InsertedRemarkItems { get; set; }

        public IEnumerable<ChattingEntity> DeletedRemarkItems { get; set; }
    
        public IEnumerable<string> InsertedSharedContacts { get; set; }
    
        public IEnumerable<string> DeletedSharedContacts { get; set; }
    }
}