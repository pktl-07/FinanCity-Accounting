namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Infrastructure;

    public class SubQueryBuilder<TEntity> : ISubQuery<TEntity>
        where TEntity : IEntity
    {
        private Select _component;
        private Expression<Func<TEntity, dynamic>> _expression;

        public SubQueryBuilder(Select component, Expression<Func<TEntity, dynamic>> expression)
        {
            _component = component;
            _expression = expression;
        }

        public void SubQuery(string template)
        {
            Guard.IsNotNullOrEmpty(template, "template");

            if (_expression.Body is UnaryExpression)
                _component.WriteSubQuery($"({template})" + $" [{((_expression.Body as UnaryExpression).Operand as MemberExpression).Member.Name}]");
            else
                _component.WriteSubQuery($"({template})" + $" [{(_expression.Body as MemberExpression).Member.Name}]");
        }
    }
}
