namespace FC.Domain.Sales.Infrastructure
{
    public class OverridingSchemeFactory
    {
        public delegate void RequireDbInsert(object objectData);
        public delegate void GetLastBatchNo(string lastBatchNo);
        public event RequireDbInsert OnRequireDbInsert;
        public event GetLastBatchNo OnGetLastBatchNo;
        private OverridingSchemeGenerator _promotionBase;

        public OverridingSchemeFactory(OverridingSchemeGenerator promotionBase)
        {
            _promotionBase = promotionBase;
        }

        public void Run(PromoteTypeEnum promoteType)
        {

            start:

            _promotionBase.PopuplateScheme();

            if (_promotionBase.HasDifferentScheme)
            {
                if (OnRequireDbInsert != null)
                    OnRequireDbInsert(_promotionBase.GetLatestScheme());

                goto start;
            }
            else
            {
                if (OnGetLastBatchNo != null)
                    OnGetLastBatchNo(_promotionBase.GetLatestBatchNo());
            }
        }
    }

}
