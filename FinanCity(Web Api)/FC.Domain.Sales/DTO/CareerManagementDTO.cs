namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class CareerManagementDTO : QueryAssert<CareerManagementDTO>
    {
        #region Ctor

        public CareerManagementDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Custom("tbl");
            RuleFor(x => x.CreateDate).Custom("tbl");
            RuleFor(x => x.EffectiveDate).Custom("tbl");
            RuleFor(x => x.SalepersonName).Custom("tbl");
            RuleFor(x => x.NRIC).Custom("tbl");
            RuleFor(x => x.CEANumber).Custom("tbl");
            RuleFor(x => x.TransType).Custom("tbl");
            RuleFor(x => x.Action).Custom("tbl");
            RuleFor(x => x.ApprovedBy).Custom("tbl");
            RuleFor(x => x.Remarks).Custom("tbl");
            RuleFor(x => x.IsDefault).Custom("tbl");
            RuleFor(x => x.Payout).Custom("tbl");
            RuleFor(x => x.From).Custom("tbl");
            RuleFor(x => x.To).Custom("tbl");
        }

        #endregion

        #region Properties

        public string SysKey { get; set; }

        public int? CreateDate { get; set; }

        public int? EffectiveDate { get; set; }

        public string SalepersonName { get; set; }

        public string Remarks { get; set; }

        public string NRIC { get; set; }

        public string ApprovedBy { get; set; }

        public string CEANumber { get; set; }

        public int TransType { get; set; }

        public string Action { get; set; }

        public decimal Payout { get; set; }

        public bool IsDefault { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        #endregion
    }
}
