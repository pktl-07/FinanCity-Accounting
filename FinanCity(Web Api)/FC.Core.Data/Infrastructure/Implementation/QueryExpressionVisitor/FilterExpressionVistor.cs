namespace FC.Core.Data.Infrastructure.Implementation
{
    using System;
    using System.Text;
    using System.Linq.Expressions;
    using FC.Core.Data.Extensions;

    /// <summary>
    /// Expression where visitor especially to use in where statement.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class FilterExpressionVistor<TEntity> 
        : BaseQueryExpressionVisitor<TEntity>, IExpressionFilter where TEntity : IEntity
    {
        public FilterExpressionVistor(Expression expression)
            : base(expression)
        {
            Visit(expression);
        }

        /// <summary>
        /// To get where statement.
        /// </summary>
        public StringBuilder WHERE { get; set; } = new StringBuilder();
        
        /// <summary>
        /// Convert binary node like <> != == to well know SQL keywork.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            WHERE.Append("(");

            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                    WHERE.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    WHERE.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    WHERE.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    WHERE.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (LIKE)
                        WHERE.Append(" LIKE ");
                    else if (node.Right.IsNullConstant())
                        WHERE.Append(" IS ");
                    else
                        WHERE.Append(" = ");
                    break;

                case ExpressionType.NotEqual:
                    if (NOTLIKE)
                        WHERE.Append(" NOT LIKE ");
                    else if (node.Right.IsNullConstant())
                        WHERE.Append(" IS NOT ");
                    else
                        WHERE.Append(" <> ");
                    break;

                case ExpressionType.LessThan:
                    WHERE.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    WHERE.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    WHERE.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    WHERE.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
            }

            Visit(node.Right);

            WHERE.Append(")");

            return node;
        }

        /// <summary>
        /// To read constance value especially in RHS eg. MyName="name" 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value ?? "NULL";
            // var value = node.Value ?? DBNull.Value;
            //var value = node.Value ?? GeneralUtility.NullConstant;
            if (LIKE || NOTLIKE)
            {
                if (STARTWITH)
                {
                    WHERE.Append(DbParameterAnnotation.ConverTo(value + "%"));
                }
                else if (ENDWITH)
                {
                    WHERE.Append(DbParameterAnnotation.ConverTo("%" + value));
                }
                else
                {
                    WHERE.Append(DbParameterAnnotation.ConverTo("%" + value + "%"));
                }

                LIKE = false;
                NOTLIKE = false;
            }
            else
            {
                WHERE.Append(DbParameterAnnotation.ConverTo(value));
            }
            return node;
        }

        /// <summary>
        /// To read member variable node especially in LHS eg. MyName="name" .
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var nodeType = node.Expression.Type;

            var instance = Activator.CreateInstance(nodeType);

            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                WHERE.Append(node.Member.Name.GetFullSQLFieldName(instance));
                return node;
            }

            if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess 
                || node.Expression.NodeType == ExpressionType.Constant)
            {
                var getter = node.CreateDynamic(typeof(object));
                return Visit(Expression.Constant(getter));
            }

            throw new NotSupportedException($"The member '{node.Member.Name}' is not supported.");
        }
    }
}
