namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;
    using FC.Domain.Sales.Entity;
  
    public class PromotionSchemeContext
    {
        public List<PayableSchemeEntity> AMS104List { get; set; }
        public List<BonusPayableEntity> AMS105List { get; set; } 
        public EmploymentEntity EMPEntity { get; set; } 
        public PersonEntity SPEntity { get; set; }
        public string PromotionMessage { get; set; }
    }
}
