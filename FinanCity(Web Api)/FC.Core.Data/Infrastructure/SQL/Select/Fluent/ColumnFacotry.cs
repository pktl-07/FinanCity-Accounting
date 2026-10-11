namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;

    public class ColumnFacotry<TEntity> : IColumnFactory<TEntity>
        where TEntity : IEntity
    {
        private Select _component;
        public ColumnFacotry(Select component)
        {
            _component = component;
        }
        
        public ISubQuery<TEntity> Column(Expression<Func<TEntity, dynamic>> expression)
        {
            _component.WriteSelector<TEntity>(expression);
            return new SubQueryBuilder<TEntity>(_component, expression);
        }

        public void Sum<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            new AggregrateColumnBuilder<TEntity>(_component).Sum(expression);
        }

        public void Count<TValue>(Expression<Func<TEntity, TValue>> expression)
        {
            new AggregrateColumnBuilder<TEntity>(_component).Count(expression);
        }

        public void Avg<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            new AggregrateColumnBuilder<TEntity>(_component).Avg(expression);
        }

        public void Max<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            new AggregrateColumnBuilder<TEntity>(_component).Max(expression);
        }

        public void Min<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct
        {
            new AggregrateColumnBuilder<TEntity>(_component).Min(expression);
        }
    }
}
