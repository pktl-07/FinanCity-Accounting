namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
   
    [Table(Name = "CommissionStructureDetail")]
    public partial class CommissionStructureDetailEntity : EntityBase,
         IReferencesConstraint
    {
        public CommissionStructureDetailEntity() : base("FC.Biz") { }

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

        [Column(Name = "ParentSK")]
        public string ParentSK { get; set; }

        [Column(Name = "Designation")]
        public string Designation { get; set; }

        [Column(Name = "BrifeDesignationName")]
        public string BrifeDesignationName { get; set; }

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

        [Column(Name = "GroupManagerComm")]
        public decimal? GroupManagerComm
        {
            get { return _GroupManagerComm ?? 0.0M; }
            set { _GroupManagerComm = value; }
        }

        [Column(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Column(Name = "Type")]
        public int Type { get; set; }

        [Column(Name = "MaxMB")]
        public int MaxMB
        {
            get { return _maxMB; }
            set { _maxMB = value; }
        }

        public void RelationWith(IReferenceFieldAnalyzer relation)
        {
            relation.Register<EmploymentEntity>(x => x.SchemeDetailSysKey);
        }
    }
}
