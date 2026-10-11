namespace FC.Core
{
    using FC.Core.Data;
    using System;
    using System.Linq.Expressions;

    public static class StringExtensions
    {
        public static Expression<Func<T, bool>> ToFilterExpr<T>(this string value) where T : IEntity
        {
            return System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(value.Trim(), new[]
            {
                Expression.Parameter(typeof(T),
                typeof(T).Name)
            });
        }
    }
}
