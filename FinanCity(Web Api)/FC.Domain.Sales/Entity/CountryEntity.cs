namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Country")]
    public class CountryEntity : EntityBase, IUniqueFieldConstraint<CountryEntity>
    {
        public CountryEntity() : base("FC.Biz") { }

        [Column(Name = "CountryCode")]
        public string CountryCode { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        public void UniqueField(IUniqueFieldAnalyzer<CountryEntity> action)
        {
            action.Register(x => new object[] { x.CountryCode });
            action.Register(x => new object[] { x.Description });
        }
    }
}
