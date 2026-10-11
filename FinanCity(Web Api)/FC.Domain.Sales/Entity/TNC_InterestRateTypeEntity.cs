namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "TNC_InterestRateType")]
    public class TNC_InterestRateTypeEntity : EntityBase
        , IUniqueFieldConstraint<TNC_InterestRateTypeEntity>
    {
        public TNC_InterestRateTypeEntity() : base("FC.Biz") { }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<TNC_InterestRateTypeEntity> action)
        {
            action.Register(f => new object[] { f.Description });
        }
    }
}
