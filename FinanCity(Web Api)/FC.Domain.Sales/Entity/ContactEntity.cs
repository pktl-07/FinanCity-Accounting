namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Contact")]
    public partial class ContactEntity : EntityBase
    {
        public ContactEntity() : base("FC.Biz") { }

        [Column(Name = "BuildingHouseNo"), Display(Name = @"Blk/House No.")]
        public string BuildingHouseNo { get; set; }

        [Column(Name = "Address1"), Display(Name = "Address")]
        public string Address1 { get; set; }

        [Column(Name = "Address2"), Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Column(Name = "FloorUnit"), Display(Name = "Floor-Unit")]
        public string FloorUnit { get; set; }

        [Column(Name = "PostalCode"), Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Column(Name = "Country"), Display(Name = "Country")]
        public string Country { get; set; }

        [Column(Name = "MailingBuildingHouseNo"), Display(Name = "MailingBuildingHouseNo")]
        public string MailingBuildingHouseNo { get; set; }

        [Column(Name = "MailingAddress1"), Display(Name = "MailingAddress1")]
        public string MailingAddress1 { get; set; }

        [Column(Name = "MailingAddress2"), Display(Name = "MailingAddress2")]
        public string MailingAddress2 { get; set; }

        [Column(Name = "MailingFloorUnit"), Display(Name = "MailingFloorUnit")]
        public string MailingFloorUnit { get; set; }

        [Column(Name = "MailingPostalCode"), Display(Name = "MailingPostalCode")]
        public string MailingPostalCode { get; set; }

        [Column(Name = "MailingCountry"), Display(Name = "Mailing Country")]
        public string MailingCountry { get; set; }

        [Column(Name = "HomeTelephone")]
        public string HomeTelephone { get; set; }

        [Column(Name = "MobilePhone")]
        public string MobilePhone { get; set; }

        [Column(Name = "MobilePhone1")]
        public string MobilePhone1 { get; set; }

        [Column(Name = "Fax")]
        public string Fax { get; set; }

        [Column(Name = "Email"), Display(Name = "Email")]
        public string Email { get; set; }

        [Column(Name = "CompanyEmail"), Display(Name = "Email")]
        public string CompanyEmail { get; set; }

        [Column(Name = "WebSite")]
        public string WebSite { get; set; }

    }
}
