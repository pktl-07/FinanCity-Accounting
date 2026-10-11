namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "SalesOrderType")]
    public partial class SalesOrderTypeEntity : EntityBase,
        IReferencesConstraint
    {
        public SalesOrderTypeEntity() : base("FC.Biz") { }

        [Column(Name = "Description"), Display(Name = ("Description"))]
        public string Description { get; set; }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "IncomeAccountSK"), Display(Name = "Account Code")]
        public string IncomeAccountSK { get; set; }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<PropertyTransactionEntity>(x => x.SaleOrderTypeSK);
        }
    }
}
