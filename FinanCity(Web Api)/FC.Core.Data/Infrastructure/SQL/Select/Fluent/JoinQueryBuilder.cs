namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class JoinQueryBuilder<TComponent> : QueryComponentBuilderBase<TComponent, JoinQueryBuilder<TComponent>>, IJoin
        where TComponent : QueryComponentBase
    {
        public JoinQueryBuilder(TComponent component) : base(component) { }
        
        public IJoin CrossJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity
        {
            Component.WriteJoin<T3, T4>(JoinType.CROSS_JOIN, predicate);
            return this;
        }

        public IJoin InnerJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity 
            where T4 : IEntity 
        {
            Component.WriteJoin<T3, T4>(JoinType.INNNER_JOIN, predicate);
            return this;
        }

        public IJoin LeftJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity 
            where T4 : IEntity 
        {
            Component.WriteJoin<T3, T4>(JoinType.LEFT_JOIN, predicate);
            return this;
        }

        public IJoin RightJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity
        {
            Component.WriteJoin<T3, T4>(JoinType.RIGHT_JOIN, predicate);
            return this;
        }

        public IHaving Where<T>(Expression<Func<T, bool>> expression) where T : IEntity
        {
            return (new WhereBuilder<TComponent>(Component) as IWhere).Where(expression);
        }

        public IHaving Where<T>(Expression<Func<T, bool>> expression, Action<IAppendix> action) where T : IEntity
        {
            return (new WhereBuilder<TComponent>(Component) as IWhere).Where(expression, action);
        }
    }
}
