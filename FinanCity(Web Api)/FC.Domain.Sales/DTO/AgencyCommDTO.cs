namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class AgencyCommDTO : QueryAssert<AgencyCommDTO>
    {
        #region Ctor

        public AgencyCommDTO() : base("FC.Biz")
        {
            RuleFor(x => x.AgencyGrossComm).Custom("tbl");
            RuleFor(x => x.AgencyGSTPercent).Custom("tbl");
            RuleFor(x => x.AgencyTotalComm).Custom("tbl");
            RuleFor(x => x.AgencyGSTValue).Custom("tbl");
            RuleFor(x => x.TotalCredit).Custom("tbl");
        }

        #endregion

        #region Properties

        public decimal AgencyGrossComm { get; set; }

        public decimal AgencyGSTPercent { get; set; }

        public decimal AgencyGSTValue { get; set; }

        public decimal AgencyTotalComm { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal TotalDebit { get; set; }

        #endregion
    }
}
