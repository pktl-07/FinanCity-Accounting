namespace FC.Domain.Sales.Infrastructure
{
    using System;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    
    /// <summary>
    /// Overriding Profile iterator item.
    /// </summary>
    public class OverridingProfileItem
    {
        #region Fields

        private Employment _employment;
        private CommissionStructureDetail _commissionStructureDetail;
        private CommissionStructure _commissionStructrue;
        private Person _person;
        private CareerManagement _careerManagement;
      //  private RecursiveSalesBonusBuilder _recursiveSalesBonusBuilder;

        #endregion

        #region Ctor

        public OverridingProfileItem(Employment employment,
          CommissionStructure commissionStructrue,
          CommissionStructureDetail commissionStructureDetail,
          //RecursiveSalesBonusBuilder recursiveSalesBonusBuilder,
          CareerManagement careerManagement,
          Person person)
        {
            // _recursiveSalesBonusBuilder = recursiveSalesBonusBuilder;
            _careerManagement = careerManagement;
            _person = person;
            _employment = employment;
            _commissionStructureDetail = commissionStructureDetail;
            _commissionStructrue = commissionStructrue;
        }

        #endregion

        #region Members

        /// <summary>
        /// Sales Order Entity.
        /// </summary>
        public PropertyTransactionEntity SalesOrder { get; internal set; }

        /// <summary>
        /// Sales Order Detail Entity.
        /// </summary>
        public PropertyTransactionDetailEntity SalesDetail { get; internal set; }

        /// <summary>
        /// Current Salesperson.
        /// </summary>
        public EmploymentEntity CurrentSalesperson
        {
            get => _employment.Finds(filter: f => f.SysKey == SalesDetail.SPSK).SingleOrDefault();
        }

        /// <summary>
        /// Tier one Salesperson info.
        /// </summary>
        public EmploymentEntity TierOne
        {
            get
            {
                var s = CurrentSalesperson?.L1;
                return _employment.Finds(filter: f => f.SysKey == s).SingleOrDefault();
            }
        }

        /// <summary>
        /// Tier two Salesperson Info.
        /// </summary>
        public EmploymentEntity TierTwo
        {
            get
            {
                var s = TierOne?.L1;
                return _employment.Finds(filter: f => f.SysKey == s).SingleOrDefault();
            }
        }

        /// <summary>
        /// Tier three Salesperson Info.
        /// </summary>
        public EmploymentEntity TierThree
        {
            get
            {
                var s = TierTwo?.L1;
                return _employment.Finds(filter: f => f.SysKey == s).SingleOrDefault();
            }
        }

        /// <summary>
        /// Tier four Salesperson Info.
        /// </summary>
        public EmploymentEntity TierFour
        {
            get
            {
                var s = TierThree?.L1;
                return _employment.Finds(filter: f => f.SysKey == s).SingleOrDefault();
            }
        }

        /// <summary>
        /// Tier five Salesperson Info.
        /// </summary>
        public EmploymentEntity TierFive
        {
            get
            {
                var s = TierFour?.L1;
                return _employment.Finds(filter: f => f.SysKey == s).SingleOrDefault();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// To get commission level for current salesperson
        /// </summary>
        /// <returns>Representaion of commission level like 1,2,3</returns>
        public string GetCommissionLevel()
        {
            var maxMBCountString = "";
            var d = _commissionStructureDetail.Finds(filter: f => f.SysKey == CurrentSalesperson.SchemeDetailSysKey).Single();
            var maxMBCount = _commissionStructrue.Finds(filter: f => f.SysKey == d.ParentSK).Single().MaxMB;

            for (int i = 1; i <= maxMBCount; i++)
            {
                if (i != maxMBCount)
                    maxMBCountString += i + ",";
                else
                    maxMBCountString += i;
            }
            return maxMBCountString;
        }

        /// <summary>
        /// To get given salesperson tier SysKey.
        /// unless he or she is not terminated or resigned.
        /// Otherwise it return reason to termination.
        /// </summary>
        /// <param name="tier">Salesperson</param>
        /// <returns>
        /// Representation of salesperson tier SysKey
        /// or termination reason.
        /// </returns>
        public string GetTierOrTerminationReason(EmploymentEntity tier)
        {
            if (tier == null)
                return null;

            var person = _person.Finds(filter: f => f.SysKey == tier.SysKey).SingleOrDefault();
            if (person.ReasonToTerminate == (int)SalepersonStatus.Live)
                return person.SysKey;

            return ((SalepersonStatus)person.ReasonToTerminate).ToDescription();
        }

        /// <summary>
        /// To find out current salesperson' s Division Manager or 
        /// his Team Manager is depending of transaction date
        /// from which salesperson' s recursive tree is achieved.
        /// </summary>
        /// <param name="rank">Rank of salesperson</param>
        /// <returns>Salesperson Tier object.</returns>
        public SalesPersonTireDTO GetSenior(RankType rank)
        {
            var downTree = _careerManagement.GetCareerManagementDownTree(SalesOrder.TransactionDate.Value);
            
            if (downTree.Where(x => x.SPSK == SalesDetail.SPSK)
                .SingleOrDefault() == null)
                throw new Exception("Invalid transaction date for this salesperson.");

            var items = _person.GetSalespersonRecursiveTree(key: SalesDetail.SPSK,
                     source: downTree,
                     downTree: false,
                     reverse: false);

            var preResult = items.RecursiveSelect(x => x.Items)
                .Where(x => x.Rank == (int)rank)
                .FirstOrDefault();

            if(preResult != null)
            return downTree.Where(x => x.SPSK == preResult.SPSK
                    && x.SPSK != SalesDetail.SPSK)
                   .SingleOrDefault();

            return null;

            //var emp = _employment.Finds(filter: f => f.SysKey == SalesDetail.SPSK).SingleOrDefault();
            //var members = _recursiveSalesBonusBuilder
            //    .Create(
            //    spsk: emp.SysKey,
            //    transactionDate: SalesOrder.TransactionDate)
            //    .GetSalesTeam();

            //if (members.Where(x => x.Key == rank).Count() > 0)
            //    return members[rank];
            //return null;
        }

        #endregion
    }
}
