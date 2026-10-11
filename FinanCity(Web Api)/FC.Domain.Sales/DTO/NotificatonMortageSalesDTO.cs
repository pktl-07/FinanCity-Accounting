namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class NotificatonMortageSalesDTO : QueryAssert<NotificatonMortageSalesDTO>
    {
        #region Ctor

        public NotificatonMortageSalesDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.CreatedDate).Custom("tbl");
            RuleFor(x => x.ReceiverSK).Custom("tbl");
            RuleFor(x => x.SenderSK).Custom("tbl");
            RuleFor(x => x.MaillingClientSK).Custom("tbl");
            RuleFor(x => x.Description).Custom("tbl");
            RuleFor(x => x.RefSK).Custom("tbl");
            RuleFor(x => x.IsRead).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
        }

        #endregion

        #region Properties

        public bool IsRead { get; set; }

        public string SysKey { get; set; }

        public int? CreatedDate { get; set; }

        public string ReceiverSK { get; set; }

        public string SenderSK { get; set; }

        public string MaillingClientSK { get; set; }

        public string Description { get; set; }

        public string RefSK { get; set; }

        public int TransType { get; set; }

        #endregion
    }
}
