namespace FC.Core.Data.Infrastructure.Implementation
{
    using System.Linq;
    using System.Reflection;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using FC.Core.Data.Extensions;

    /// <summary>
    /// Expression selector visitor especially for the select statement.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ReaderExpressionVisitor<TEntity> : BaseQueryExpressionVisitor<TEntity>, IExpressionSelector where TEntity : IEntity
    {
        private bool _requiredMemberInfo;
        public ReaderExpressionVisitor(Expression expression, bool requiredMemberInfo) : base(expression)
        {
            _requiredMemberInfo = requiredMemberInfo;
            Visit(expression);
        }

        /// <summary>
        /// Field collection.
        /// </summary>
        public ICollection<string> FieldCollection { get; set; } = new List<string>();

        /// <summary>
        /// To read member variable.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (_requiredMemberInfo)
                FieldCollection.Add($"{node.Member.Name.GetFullSQLFieldName(Entity)} [{node.Member.Name}]");
            else
                FieldCollection.Add($"{node.Member.Name.GetFullSQLFieldName(Entity)}");

            return node;
        }

        /// <summary>
        /// To read Lambda exprssion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            bool found = false;

            var memberInfo = node.Body.Type.GetRuntimeProperties()
                .Where(x => !x.Name.Contains("FirstChar")
                && !x.Name.Contains("Chars")
                && !x.Name.Contains("Length")).ToList();

            for (int i = 0; i < memberInfo.Count; i++)
            {
                found = true;
                if (_requiredMemberInfo)
                    FieldCollection.Add($"{memberInfo[i].Name.GetFullSQLFieldName(Entity)} [{memberInfo[i].Name}]");
                else
                    FieldCollection.Add($"{memberInfo[i].Name.GetFullSQLFieldName(Entity)}");
            }

            if (!found)
                return Visit(node.Body);

            return node;
        }
    }
}
