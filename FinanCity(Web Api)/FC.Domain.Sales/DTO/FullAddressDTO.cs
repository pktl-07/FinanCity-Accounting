namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class FullAddressDTO : QueryAssert<FullAddressDTO>
    {
        #region Ctor

        public FullAddressDTO() : base("FC.Biz")
        {
            RuleFor(x => x.Blk).Custom("tbl");
            RuleFor(x => x.Address1).Custom("tbl");
            RuleFor(x => x.Address2).Custom("tbl");
            RuleFor(x => x.BuildingSK).Custom("tbl");
            RuleFor(x => x.StreetSK).Custom("tbl");
            RuleFor(x => x.AddressType).Custom("tbl");
            RuleFor(x => x.TypeFlag).Custom("tbl");
            RuleFor(x => x.PostalCode).Custom("tbl");
        }

        #endregion

        #region Properties

        public string Blk { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string BuildingSK { get; set; }
        public string StreetSK { get; set; }
        public string AddressType { get; set; }
        public string TypeFlag { get; set; }
        public string PostalCode { get; set; }

        #endregion
    }
}
