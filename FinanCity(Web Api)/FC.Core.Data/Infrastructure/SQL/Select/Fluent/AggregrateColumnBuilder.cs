namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;

    public class AggregrateColumnBuilder<TEntity> : IAggregrate<TEntity>
        where TEntity : IEntity
    {
        private Select _component;

        public AggregrateColumnBuilder(Select component)
        {
            _component = component;
        }

        public void Sum<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            _component.WriteAggregate<TEntity>(expression, AggregrateType.Sum);
        }

        public void Count<TValue>(Expression<Func<TEntity, TValue>> expression) 
        {
            _component.WriteAggregate<TEntity>(expression, AggregrateType.Count);
        }

        public void Avg<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            _component.WriteAggregate<TEntity>(expression, AggregrateType.Avg);
        }

        public void Max<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            _component.WriteAggregate<TEntity>(expression, AggregrateType.Max);
        }

        public void Min<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            _component.WriteAggregate<TEntity>(expression, AggregrateType.Min);
        }
    }
}
