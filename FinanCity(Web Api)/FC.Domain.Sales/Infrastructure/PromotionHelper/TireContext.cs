namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
 
    public class TireContext : BaseCommissionHelper
    {
        #region Fields

        private OverridingProfile _overridingProfile;
        private OverridingScheme _overridingScheme;
        private decimal _closeSalesToDate; // CLOSE SALES TO DATE
        private decimal _grossToPromote;
        private OverridingSchemeEntity _currentScheme;
        private CommissionHelper _commissionHelper;
      
        #endregion

        #region Ctor

        public TireContext(Person person,
         Employment employment,
         OverridingScheme overridingScheme,
         CommissionHelper commissionHelper,
         CommissionStructureDetail commissionStructureDetail,
         OverridingProfile overridingProfile,
         IEssAppRepository app,
         PropertyTransactionDetail salesOrderDetail)
         : base(person,
               employment,
               overridingScheme,
               commissionStructureDetail,
               overridingProfile,
               salesOrderDetail)
        {
            this._commissionHelper = commissionHelper;
            this._overridingProfile = overridingProfile;
            this._overridingScheme = overridingScheme;
        }

        #endregion

        #region Methods

        public override CommissionContext GetContext(string invoiceSK,
            string saleOrderDetailSK,
            string spsk,
            decimal allocatedAmt,
            string alosk,
            string batchNo,
            decimal grossToPromoteNextLevel)
        {
            var result = base.GetContext(invoiceSK,
                 saleOrderDetailSK,
                 spsk,
                 allocatedAmt,
                 alosk,
                 batchNo,
                 grossToPromoteNextLevel: grossToPromoteNextLevel);

            _currentScheme = _overridingScheme.Finds(filter: f => f.BatchNo == batchNo
                  && f.SchemeSK == result.EmploymentEntity.SchemeDetailSysKey).SingleOrDefault();

            _grossToPromote = grossToPromoteNextLevel;

            _closeSalesToDate = _commissionHelper.GetCloseSalesToDate(spsk: spsk,
            invoicesk: invoiceSK);

            var balance = allocatedAmt.ToRoundUp();
            var payable = new List<PayableSchemeEntity>();
            var bonus = new List<BonusPayableEntity>();

            while (balance.ToRoundUp() != 0)
            {
                if (IsLastScheme())
                {
                    BuildPayableScheme(balance.ToRoundUp(), payable);

                    balance = 0; // FORCE TO TERMINATE LOOPING

                    break;
                }
                if (_closeSalesToDate >= _currentScheme.MinCom && _closeSalesToDate < _currentScheme.MaxCom)
                {
                    BuildPayableScheme(balance.ToRoundUp(), payable);

                    balance = 0; // FORCE TO TERMINATE LOOPING

                    break;
                }
                if (_closeSalesToDate == _currentScheme.MaxCom)
                {
                    BuildPayableScheme(balance.ToRoundUp(), payable);

                    UpdateCurrentScheme(result.EmploymentEntity);

                    balance = 0; // FORCE TO TERMINATE LOOPING

                    break;
                }
                if (_closeSalesToDate > _currentScheme.MaxCom)
                {
                    BuildPayableScheme(_grossToPromote.ToRoundUp(), payable);

                    balance -= _grossToPromote.ToRoundUp();

                    UpdateCurrentScheme(result.EmploymentEntity);
                }
            }

            result.AMS104List = payable;

            BuildBonusScheme(result.AMS104List, bonus);

            result.AMS105List = bonus;

            return result;
        }

        #endregion

        #region Utilities

        private void BuildPayableScheme(decimal baseValue,
          List<PayableSchemeEntity> payableResult)
        {
            if (baseValue.ToRoundUp() < 0)
            {
                throw new Exception($@"Base value should not be negative value,
                     please check in the program code, the suggestion is that the Gross To Promote Next Level variable should be assigned before CRUD function call");
            }

            var spsk = SPSK;
            var ptosk = InvoiceSK;
            var batchno = BatchNo;

            var overridesk = _overridingProfile.Finds(filter: f => f.SPSK == spsk && f.PTOSK == ptosk).Single().SysKey;
            var schemesk = _overridingScheme.Finds(filter: f => f.SchemeSK == _currentScheme.SchemeSK && f.BatchNo == batchno).Single().SysKey;

            payableResult.Add(new PayableSchemeEntity
            {
                SysKey = GeneralUtility.Key,
                RecordStatus = (int)RecordStatus.Post,
                ALOSK = ALOSK,
                GrossComm = baseValue.ToRoundUp(),
                AgencyPercent = _currentScheme.AgencyPercent ?? 0.00M,
                AssociatePercent = _currentScheme.AssociatedPercent ?? 0.00M,
                AgencyComm = ((baseValue / 100) * _currentScheme.AgencyPercent ?? 0.0M).ToRoundUp(),
                AssociateComm = ((baseValue / 100) * _currentScheme.AssociatedPercent ?? 0.0M).ToRoundUp(),
                SPSK = SPSK,
                InvoiceSK = InvoiceSK,
                //JUN003
                OverrideSchemeProfileSK = overridesk,
                //JUN004
                OverrideSchemeSK = schemesk
            });
        }

        private bool IsLastScheme()
        {
            if (SchemeDetailEnties.Last().SchemeSK == _currentScheme.SchemeSK)
                return true;
            else
                return false;
        }

        private void UpdateCurrentScheme(EmploymentEntity employeeEntity)
        {
            if (!IsLastScheme())
            {
                _currentScheme = SchemeDetailEnties.Where(x => x.MaxCom > _currentScheme.MaxCom).FirstOrDefault(); // REDEFINE CURRENT SCHEME

                var previousScheme = SchemeDetailEnties.Where(x => x.MaxCom < _currentScheme.MaxCom).OrderByDescending(x => x.MaxCom).FirstOrDefault();

                if (previousScheme != null)
                    _grossToPromote = _currentScheme.MaxCom - previousScheme.MaxCom; // RE DEFINE GTP

                employeeEntity.SchemeDetailSysKey = _currentScheme.SchemeSK; // REDEFINE CURRENT SCHEME
            }
            else
            {
                _grossToPromote = 0;
            }
        }

        #endregion
    }
}
