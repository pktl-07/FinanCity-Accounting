namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Business;

    public class OverridingSchemeGenerator : PromotionSchmeFactoryBase
    {
        private IList<CommissionStructureDetailEntity> _currentScheme;
        private IList<OverridingSchemeEntity> _lastScheme;

        private CommissionStructureDetail _commissionSchemeDetail;
        private OverridingScheme _overridingScheme;

        public OverridingSchemeGenerator(CommissionStructureDetail commissionScheme,
            OverridingScheme overridingScheme)
        {
            _commissionSchemeDetail = commissionScheme;
            _overridingScheme = overridingScheme;
        }

        public override void PopuplateScheme()
        {
            _currentScheme = _commissionSchemeDetail.Finds(filter: f => f.IsActive == true).OrderBy(x => x.MinCom).ToList();
            _lastScheme = _overridingScheme.FindLastBatch().ToList() ?? new List<OverridingSchemeEntity>();
        }

        public override bool HasDifferentScheme
        {
            get
            {
                if (_currentScheme.Count() != _lastScheme.Count())
                    return true;

                for (int index = 0; index < _currentScheme.Count(); index++)
                {
                    if (_currentScheme[index].ParentSK != _lastScheme[index].SchemeHeaderSK)
                        return true;

                    if (_currentScheme[index].AssociatedPercent != _lastScheme[index].AssociatedPercent)
                        return true;

                    if (_currentScheme[index].AgencyPercent != _lastScheme[index].AgencyPercent)
                        return true;

                    if (_currentScheme[index].MaxCom != _lastScheme[index].MaxCom)
                        return true;

                    if (_currentScheme[index].MinCom != _lastScheme[index].MinCom)
                        return true;

                    if (_currentScheme[index].L1 != _lastScheme[index].L1)
                        return true;

                    if (_currentScheme[index].L2 != _lastScheme[index].L2)
                        return true;

                    if (_currentScheme[index].L3 != _lastScheme[index].L3)
                        return true;

                    if (_currentScheme[index].L4 != _lastScheme[index].L4)
                        return true;

                    if (_currentScheme[index].L5 != _lastScheme[index].L5)
                        return true;

                    if (_currentScheme[index].MaxMB != _lastScheme[index].MaxMB)
                        return true;

                    if (_currentScheme[index].Type != _lastScheme[index].Type)
                        return true;
                }

                return false;
            }
        }

        public override object GetLatestScheme()
        {
            _lastScheme.Clear();
            
            var batchNo = GeneralUtility.Key;

            _currentScheme.Each(x =>
            {
                _lastScheme.Add(new OverridingSchemeEntity
                {
                    BatchNo = batchNo,
                    SchemeHeaderSK = x.ParentSK,
                    SchemeSK = x.SysKey,
                    AgencyPercent = x.AgencyPercent,
                    AssociatedPercent = x.AssociatedPercent,
                    MinCom = x.MinCom,
                    MaxCom = x.MaxCom,
                    L1 = x.L1,
                    L2 = x.L2,
                    L3 = x.L3,
                    L4 = x.L4,
                    L5 = x.L5,
                    MaxMB = x.MaxMB,
                    Type = x.Type,
                });
            });

            return _lastScheme;
        }

        public override string GetLatestBatchNo()
        {
            return _lastScheme.Last().BatchNo;
        }
    }
}
