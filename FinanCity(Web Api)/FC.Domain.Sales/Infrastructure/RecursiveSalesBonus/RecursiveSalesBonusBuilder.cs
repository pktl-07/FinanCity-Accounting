//namespace FC.Domain.Sales.Infrastructure
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using FC.Core;
//    using FC.Domain.Sales.Business;
//    using FC.Domain.Sales.DTO;
//    using Ninject;

//    /// <summary>
//    ///To be find out individual salesperson 
//    ///for his senior ranking people depending on the specific date.
//    /// </summary>
//    public class RecursiveSalesBonusBuilder
//    {
//        #region Fields

//        private string _spsk;
//        private int? _transactionDate;
//        private CareerManagement _careerManagement;
//        private RankType[] _ranks;

//        #endregion

//        #region Ctor

//        public RecursiveSalesBonusBuilder(RankType [] ranks,
//            CareerManagement careerManagement)
//        {
//            _ranks = ranks;
//            _careerManagement = careerManagement;
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// To crate recursive sales bonus for specific salesperson
//        /// depends on the transaction date.
//        /// </summary>
//        /// <param name="spsk">Salesperson key.</param>
//        /// <param name="transactionDate">Transaction date.</param>
//        /// <returns>Recursive Sales Bonus</returns>
//        public IRecursiveSalesBonus Create(string spsk,
//          int? transactionDate)
//        {
//            _spsk = spsk;
//            _transactionDate = transactionDate;

//            DI.Current.Rebind<IRecursiveSalesBonus>()
//                .To<FinanCitySalesBonus>()
//                .WithConstructorArgument("context", BuildContext());

//            return DI.Current.Get<IRecursiveSalesBonus>();
//        }

//        #endregion

//        #region Utilities

//        private RecursiveSalesContext BuildContext()
//        {
//            var context = new RecursiveSalesContext();
//            var source = _careerManagement.GetCareerManagementDownTree(_transactionDate.Value);

//            Validate(source, () =>
//            {
//                context.Source = source;
//                context.Target = BuildTarget(source: source);

//                //In future here is a entry point to increase the 
//                //ranks to provide for the sales bonus. Example RankType.Officer
//                //put int the array list.
//                context.Ranks = BuildRanks(ranks: _ranks,
//                   target: context.Target);
//            });

//            return context;
//        }

//        private void Validate(IEnumerable<SalesPersonTireDTO> source,
//            Action success)
//        {
//            if (source == null ||
//                source.Where(x => x.SPSK == _spsk).Count() == 0)
//                throw new Exception($@"Invalid transaction date for salesperson.
//                Reason: He did not start work at {GeneralUtility.ConvertDisplayDateFormat(_transactionDate)}.");
         
//            success();
//        }

//        private SalesPersonTireDTO BuildTarget(IEnumerable<SalesPersonTireDTO> source)
//        {
//            return source.Where(x => x.SPSK == _spsk).Last();
//        }

//        private int[] BuildRanks(RankType[] ranks,
//            SalesPersonTireDTO target)
//        {
//            return ranks
//                .Select(x => (int)x)
//                .Where(rank => rank > target.Rank)
//                .OrderByDescending(rank => rank)
//                .ToArray();
//        }

//        #endregion
//    }
//}
