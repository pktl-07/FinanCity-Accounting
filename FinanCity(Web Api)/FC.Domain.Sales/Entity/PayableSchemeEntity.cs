namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS104")]
    public class PayableSchemeEntity : EntityBase
    {
        public PayableSchemeEntity() : base("FC.Biz") { }

        [Column(Name = "ALOSK")]
        public string ALOSK
        {
            get;
            set;
        }

        [Column(Name = "GrossComm"), MultiCurrency]
        public decimal GrossComm
        {
            get;
            set;
        }

        [Column(Name = "AgencyPercent")]
        public decimal AgencyPercent
        {
            get;
            set;
        }

        [Column(Name = "AssociatePercent")]
        public decimal AssociatePercent
        {
            get;
            set;
        }

        [Column(Name = "AgencyComm"), MultiCurrency]
        public decimal AgencyComm
        {
            get;
            set;
        }

        [Column(Name = "AssociateComm"), MultiCurrency]
        public decimal AssociateComm
        {
            get;
            set;
        }

        [Column(Name = "OverrideSchemeSK")]
        public string OverrideSchemeSK
        {
            get;
            set;
        }

        [Column(Name = "OverrideSchemeProfileSK")]
        public string OverrideSchemeProfileSK
        {
            get;
            set;
        }

        [Column(Name = "SPSK")]
        public string SPSK
        {
            get;
            set;
        }

        [Column(Name = "InvoiceSK")]
        public string InvoiceSK
        {
            get;
            set;
        }
    }
}
