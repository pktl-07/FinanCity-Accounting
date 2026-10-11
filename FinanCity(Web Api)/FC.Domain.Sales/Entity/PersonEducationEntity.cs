namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "PersonEducation")]
    public partial class PersonEducationEntity : EntityBase
    {
        public PersonEducationEntity() : base("FC.Biz") { }

        [Column(Name = "SPSysKey")]
        public string SPSysKey { get; set; }

        [Column(Name = "EduSysKey")]
        public string EduSysKey { get; set; }
    }
}
