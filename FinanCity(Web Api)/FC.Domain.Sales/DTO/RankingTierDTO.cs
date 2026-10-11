namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;

    public class RankingTierDTO : QueryAssert<RankingTierDTO>
    {
        #region Ctor

        public RankingTierDTO() : base("FC.Biz")
        {
            RuleFor(x => x.TireId).Custom("tbl");
            RuleFor(x => x.TireSK).Custom("tbl");
            RuleFor(x => x.Rank).Custom("tbl");
            RuleFor(x => x.Percentage).Custom("tbl");
        }

        #endregion

        #region Properties

        public int TireId { get; set; }
        public string TireSK { get; set; }
        public int Rank { get; set; }
        public decimal Percentage { get; set; }

        #endregion
    }
}
