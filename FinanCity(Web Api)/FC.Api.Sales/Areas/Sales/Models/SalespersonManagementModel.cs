namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;
    using System.Collections.Generic;

    public class SalespersonManagementModel
    {
        public PersonEntity Person { get; set; }
       
        public ContactEntity Contact { get; set; }
        
        public EmploymentEntity Employment { get; set; }
        
        public EmploymentHistoryEntity EmploymentHistory { get; set; }
        
        public IEnumerable<IndustryExamEntity> Industry { get; set; }
        
        public IEnumerable<QualificationEntity> Qualification { get; set; }

        public PayNowEntity PayNow { get; set; }
    }
}