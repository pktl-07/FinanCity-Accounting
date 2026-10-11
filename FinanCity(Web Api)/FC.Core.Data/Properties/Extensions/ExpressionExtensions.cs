namespace ESS.Web.Data.Extensions
{
    using System;
    using System.Linq.Expressions;

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
    }
}
