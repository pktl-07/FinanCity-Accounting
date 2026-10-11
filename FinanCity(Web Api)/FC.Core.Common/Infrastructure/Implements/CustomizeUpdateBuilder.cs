namespace FC.Core.Infrastructure.Implements
{
    using System;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core.Data.SQL;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure.Implements.RecordIndexer;

    public class CustomizeUpdateBuilder : Update
    {
        #region Fields

        private RecordIndexerFactory _recordIndexer;
        private ExpressionAnalyzer _expressionAnalyzer;
        private IMultiCurrencyConverter _currencyConverter;
        private EntityBase _entity;

        #endregion

        #region Ctor

        public CustomizeUpdateBuilder(RecordIndexerFactory recordIndexer,
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
            var own = Activator.CreateInstance<T>();
            var instance = Expression.Lambda<Func<T, T>>((expr as LambdaExpression).Body, (expr as LambdaExpression).Parameters).Compile()(own);
            _entity = (instance as EntityBase);
            _expressionAnalyzer.Subscribe<T>(expr); // start trigger

            #region Set Time Stamp

            _entity.ModifiedDate = GeneralUtility.ServerDate;
            _entity.ModifiedTime = GeneralUtility.ServerTime;

            #endregion

            if (_expressionAnalyzer.FieldCollection.Count
                == own.GetType().GetProperties().Length)
            {
                #region Set Serail Number

                var indexer = _entity.GetIndexerAttributes();
                if (indexer != null)
                {
                    if (indexer.AutoGenerateIdReferencesOnPost &&
                        _entity.R1 == CommonConstant.DefaultIndexing.R1 &&
                        _entity.RecordStatus >= (int)RecordStatus.Post)
                    {
                        var setting = _recordIndexer.Generate((TransType)_entity.GetTableType(), _entity) ?? new RecordIndexrContext();

                        if (_entity.RecordStatus == (int)RecordStatus.Save || _entity.RecordStatus >= (int)RecordStatus.Post)
                        {
                            _entity.R1 = setting.R1;
                            _entity.Prefix = setting.Prefix;
                            _entity.Postfix = setting.Postfix;
                            _entity.VoucherId = setting.VoucherId;
                        }
                    }
                }

                #endregion

                #region Set Multi Currency Converter

                var obj = instance.CloneJson();

                if (instance is IMultiCurrency)
                    _currencyConverter.ToBaseCurrency(obj);
                else
                    _currencyConverter.ToCurrency(obj);

                #endregion

                Expression<Func<T, T>> newExperssion = (x) => obj;

                base.WriteSelector<T>(newExperssion);

                return;
            }

            base.WriteSelector<T>(expr);
        }

        public override void WriteWhere<T>(Expression expr)
        {
            base.WriteWhere<T>(expr);
            var constraint = DI.Current.Get<IUniqueFieldAnalyzer<T>>();

            #region Set Unique Key Constraint

            if (_entity is IUniqueFieldConstraint<T>)
            {

                (_entity as IUniqueFieldConstraint<T>)
                    .UniqueField(constraint);

                constraint.ValidateOnUpdate<T>(
                    fields: _expressionAnalyzer.FieldCollection,
                    filter: base.Where.ToString());
            }

            #endregion
        }

        #endregion
    }
}
