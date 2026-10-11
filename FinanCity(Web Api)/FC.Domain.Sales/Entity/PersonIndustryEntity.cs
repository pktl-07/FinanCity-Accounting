namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "PersonIndustryExam")]
    public partial class PersonIndustryEntity : EntityBase
    {
        public PersonIndustryEntity() : base("FC.Biz") { }

        [Column(Name = "SPSysKey")]
        public string SPSysKey { get; set; }

        [Column(Name = "IndusSysKey")]
        public string IndusSysKey { get; set; }
    }
}
