namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "IDIndexing")]
    public class IDIndexingEntity : EntityBase
    {
        public IDIndexingEntity() : base("FC.Biz") { }

        [Column(Name = "StartValue"), Display(Name = "Start Value")]
        public int StartValue { get; set; }

        [Column(Name = "EndValue"), Display(Name = "End Value")]
        public int EndValue { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "Format")]
        public int Format { get; set; }

        [Column(Name = "BaseDate")]
        public string BaseDate { get; set; }
    }
}
