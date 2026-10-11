namespace FC.Core
{
    using System.Linq.Expressions;

    public static class ExpressionExtensions
    {
        public static MemberExpression ToMemberExpression(this LambdaExpression expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;

                if (unaryExpression != null)
                    memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression;
        }

        public static bool IsNullConstant(this Expression expression)
        {
            return (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null);
        }
    }
}
