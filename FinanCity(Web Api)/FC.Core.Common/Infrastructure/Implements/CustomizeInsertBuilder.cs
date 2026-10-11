namespace FC.Core.Infrastructure.Implements
{
    using System;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.SQL;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
 
    public class CustomizeInsertBuilder : Insert
    {
        #region Fields

        private RecordIndexerFactory _recordIndexer;
        private ExpressionAnalyzer _expressionAnalyzer;
        private IMultiCurrencyConverter _currencyConverter;

        #endregion

        #region Ctor

        public CustomizeInsertBuilder(RecordIndexerFactory recordIndexer,
           ExpressionAnalyzer expressionAnalyzer,
           IMultiCurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
            _recordIndexer = recordIndexer;
            _expressionAnalyzer = expressionAnalyzer;
        }

        #endregion

        #region Methods

        public override void WriteSelector<T>(Expression expr)
        {
            var unitConstaint = DI.Current.Get<IUniqueFieldAnalyzer<T>>();

            var instance = Expression.Lambda<Func<T, T>>((expr as LambdaExpression).Body, (expr as LambdaExpression).Parameters).Compile()(Activator.CreateInstance<T>());
            var entity = (instance as EntityBase);

            _expressionAnalyzer.Subscribe<T>(expr);

            if (entity.RecordStatus == (int)RecordStatus.None)
                entity.RecordStatus = (int)RecordStatus.Save;

            #region Set Unique Constraint

            if (entity is IUniqueFieldConstraint<T>)
            {
                (entity as IUniqueFieldConstraint<T>).UniqueField(unitConstaint);
                unitConstaint.ValidateOnSave(instance);
            }

            #endregion

            #region Set Time Stamp

            if (entity.CreatedDate == null)
                entity.CreatedDate = GeneralUtility.ServerDate;

            if (entity.CreatedTime == null)
                entity.CreatedTime = GeneralUtility.ServerTime;

            #endregion

            #region Set Serial Key

            var indexer = entity.GetIndexerAttributes();
            var tableName = entity.GetType().Name;
            var dy = (dynamic)entity;

            if (indexer != null
                && !(tableName == "PersonEntity"
                && dy.TransType == (int)TransType.Lead))
            {
                if ((indexer.AutoGenerateIdReferencesOnSave || indexer.AutoGenerateIdReferencesOnPost)
                    && entity.R1 == CommonConstant.DefaultIndexing.R1
                    && (entity.RecordStatus == (int)RecordStatus.Save || entity.RecordStatus == (int)RecordStatus.Post))
                {
                    var setting = _recordIndexer.Generate((TransType)entity.GetTableType(), entity) ?? new RecordIndexrContext();

                    entity.R1 = setting.R1;
                    entity.Prefix = setting.Prefix;
                    entity.Postfix = setting.Postfix;
                    entity.VoucherId = setting.VoucherId;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(entity.R1))
                {
                    entity.R1 = CommonConstant.DefaultIndexing.R1;
                }
            }

            #endregion

            #region Set Multi Currency

            var obj = instance.CloneJson();

            if (instance is IMultiCurrency)
                _currencyConverter.ToBaseCurrency(obj);
            else
                _currencyConverter.ToCurrency(obj);

            #endregion

            Expression<Func<T, T>> newExperssion = (x) => obj;

            base.WriteSelector<T>(newExperssion);
        }

        #endregion
    }
}
