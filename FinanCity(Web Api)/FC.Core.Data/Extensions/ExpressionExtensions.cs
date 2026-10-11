namespace FC.Core.Data.Extensions
{
    using System;
    using System.Linq.Expressions;
    using Infrastructure;
    using Infrastructure.Implementation;
   
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Check expression is a null value.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static bool IsNullConstant(this Expression expression)
        {
            return (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null);
        }

        /// <summary>
        /// Create dynamic data for the giving expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateDynamic(this Expression expression, Type type)
        {
            return Expression.Lambda<Func<object>>(Expression.Convert(expression, type)).Compile().DynamicInvoke();
        }

        /// <summary>
        /// Create Constance value in the giving excpression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">Property of type entity or datatable.</param>
        /// <param name="right">Value of constance value.</param>
        /// <returns>Expression of constance for giving expression</returns>
        public static Expression<Func<T, bool>> Constant<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) where T : IEntity
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(Expression.Invoke(left, param), Expression.Invoke(right, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Create Sql for the giving expression.
        /// </summary>
        /// <typeparam name="T">Type of entity or datatable</typeparam>
        /// <param name="expression">Expression for filter</param>
        /// <returns>SQL String</returns>
        public static string ToSQL<T>(this Expression<Func<T, bool>> expression) where T : IEntity
        {
            try
            {
                return (new FilterExpressionVistor<T>(expression) as IExpressionFilter)
                    .WHERE.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
