namespace FC.Domain.Sales.Entity
{
    using System.ComponentModel.DataAnnotations;
    using FC.Core;
    using FC.Core.Data;
  
    [Table(Name = "Lead")]
    public class LeadEntity : EntityBase,
         IReferencesConstraint
    {
        public LeadEntity() : base("FC.Biz") { }

        [Column(Name = "Source"), Display(Name = "Source")]
        public string Source { get; set; }

        [Column(Name = "PropertyTypeSK"), Display(Name = "Property Type")]
        public string PropertyTypeSK { get; set; }

        [Column(Name = "SalesActivity"), Display(Name = "Sales Activity")]
        public string SalesActivity { get; set; }

        [Column(Name = "LeadCloseRate"), Display(Name = "Lead Close Rate")]
        public int LeadCloseRate { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PTOClientEntity>(x => x.ClientSK);
        }
    }
}
