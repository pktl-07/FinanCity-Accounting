namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
   
    [Table(Name = "JUN004")]
    public class OverridingSchemeEntity : EntityBase
    {
        public OverridingSchemeEntity() : base("FC.Biz") { }

        #region Fields

        private decimal? _AssociatedPercent;
        private decimal? _AgencyPercent;
        private decimal? _L1;
        private decimal? _L2;
        private decimal? _L3;
        private decimal? _L4;
        private decimal? _L5;
        private decimal? _GroupManagerComm;

        private int _maxMB;

        #endregion

        #region Properties

        [Column(Name = "BatchNo")]
        public string BatchNo { get; set; }

        [Column(Name = "SchemeSK")]
        public string SchemeSK { get; set; }

        [Column(Name = "SchemeHeaderSK")]
        public string SchemeHeaderSK { get; set; }

        [Column(Name = "MinCom")]
        public decimal MinCom { get; set; }

        [Column(Name = "MaxCom")]
        public decimal MaxCom { get; set; }

        [Column(Name = "AssociatedPercent")]
        public decimal? AssociatedPercent
        {
            get { return _AssociatedPercent ?? 0.00M; }
            set { _AssociatedPercent = value; }
        }

        [Column(Name = "AgencyPercent")]
        public decimal? AgencyPercent
        {
            get { return _AgencyPercent ?? 0.0M; }
            set { _AgencyPercent = value; }
        }

        [Column(Name = "L1")]
        public decimal? L1
        {
            get { return _L1 ?? 0.0M; }
            set { _L1 = value; }
        }

        [Column(Name = "L2")]
        public decimal? L2
        {
            get { return _L2 ?? 0.0M; }
            set { _L2 = value; }
        }

        [Column(Name = "L3")]
        public decimal? L3
        {
            get { return _L3 ?? 0.0M; }
            set { _L3 = value; }
        }

        [Column(Name = "L4")]
        public decimal? L4
        {
            get { return _L4 ?? 0.0M; }
            set { _L4 = value; }
        }

        [Column(Name = "L5")]
        public decimal? L5
        {
            get { return _L5 ?? 0.00M; }
            set { _L5 = value; }
        }

        [Column(Name = "MaxMB")]
        public int MaxMB
        {
            get { return _maxMB; }
            set { _maxMB = value; }
        }

        [Column(Name = "Type")]
        public int Type
        {
            get;
            set;
        }

        #endregion
    }
}
