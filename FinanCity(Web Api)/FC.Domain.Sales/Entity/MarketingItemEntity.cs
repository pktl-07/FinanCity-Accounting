namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "PromotionItem")]
    public partial class MarketingItemEntity : EntityBase,
        IUniqueFieldConstraint<MarketingItemEntity>,
        IReferencesConstraint
    {
        public MarketingItemEntity() : base("FC.Biz") { }

        [Column(Name = "Description"), Display(Name = ("Promotion Item"))]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "Price")]
        public decimal Price { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<MarketingExpenseEntity>(x => x.ItemSK);
        }

        public void UniqueField(IUniqueFieldAnalyzer<MarketingItemEntity> action)
        {
            action.Register(x => new object[] { x.Description });
        }
    }
}
