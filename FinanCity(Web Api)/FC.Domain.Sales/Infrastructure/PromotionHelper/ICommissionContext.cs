namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;
    using FC.Domain.Sales.Entity;
   
    public class CommissionContext
    {
        public IEnumerable<PayableSchemeEntity> AMS104List { get; set; }
        public IEnumerable<BonusPayableEntity> AMS105List { get; set; }
        public EmploymentEntity EmploymentEntity { get; set; }
        public PersonEntity SalespersonEntity { get; set; }
        public string PromotionMessage { get; set; }
    }
}
