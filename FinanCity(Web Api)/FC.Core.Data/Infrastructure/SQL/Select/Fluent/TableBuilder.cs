namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class TableBuilder<TComponent,T> : QueryComponentBuilderBase<TComponent, TableBuilder<TComponent,T>>, ITable
     where TComponent : QueryComponentBase
    {
        public TableBuilder(TComponent component) : base(component) { }

        public IJoin CrossJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate1)
            where T3 : IEntity
            where T4 : IEntity
        {
            return new JoinQueryBuilder<TComponent>(Component).CrossJoin(predicate1);
        }

        public IJoin InnerJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate1)
            where T3 : IEntity 
            where T4 : IEntity 
        {
            return new JoinQueryBuilder<TComponent>(Component).InnerJoin(predicate1);
        }

        public IJoin LeftJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate1)
            where T3 : IEntity
            where T4 : IEntity 
        {
            return new JoinQueryBuilder<TComponent>(Component).LeftJoin(predicate1);
        }

        public IJoin RightJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate1)
            where T3 : IEntity
            where T4 : IEntity
        {
            return new JoinQueryBuilder<TComponent>(Component).RightJoin(predicate1);
        }

        public IHaving Where<T1>(Expression<Func<T1, bool>> expression) where T1 : IEntity
        {
            return (new WhereBuilder<TComponent>(Component) as IWhere).Where(expression);
        }

        public IHaving Where<T1>(Expression<Func<T1, bool>> expression, Action<IAppendix> action) where T1 : IEntity
        {
            return (new WhereBuilder<TComponent>(Component) as IWhere).Where(expression, action);
        }
    }
}
