namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "TNC_Duration")]
    public class TNC_DurationEntity : EntityBase,
         IReferencesConstraint
    {
        public TNC_DurationEntity() : base("FC.Biz") { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<TNC_Entity>(x => x.InterestRateTypeDurationSK);
        }
    }
}
