namespace FC.Core.Infrastructure.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;

    public class ExpressionAnalyzer : ExpressionVisitor
       
    {
        private string _memberName;
        private Type _type;

        public void Subscribe<T>(Expression node) where T : IEntity
        {
            _type = typeof(T);
            base.Visit(node);
        }

        public IDictionary<string, object> FieldCollection { get; private set; } = new Dictionary<string, object>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var getter = node.CreateDynamic(typeof(object));
         
            Visit(Expression.Constant(getter, getter.GetType()));
            return node;

            throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Body.NodeType == ExpressionType.MemberInit)
                return Visit(node.Body);
            else if (node.Body.NodeType == ExpressionType.MemberAccess)
            {
                var dy = node.Body.CreateDynamic(typeof(object));
                dy.GetType().GetProperties().Each(x =>
                {
                    FieldCollection.Add(x.Name, $"{x.GetValue(dy)}");
                });
            }
            return node;
        }
        
        protected override Expression VisitConstant(ConstantExpression node)
        {
            FieldCollection.Add(_memberName, $"{node.Value}");

            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            if (node.BindingType == MemberBindingType.Assignment)
                _memberName = node.Member.Name;

            return base.VisitMemberAssignment(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                _memberName = node.Member.Name;
                return node;
            }

            if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess
                || node.Expression.NodeType == ExpressionType.Constant)
            {
                var getter = node.CreateDynamic(typeof(object));
                return Visit(Expression.Constant(getter));
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);

            return node;
        }
    }
}
