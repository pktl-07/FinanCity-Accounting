namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
  
    public static class ExpressionExtensions
    {
        public static T CreateObject<T>(this Expression<Func<T, bool>> value,
            Action<string, string> func = null)
            where T : IEntity
        {
            var visitor = new FilterExperssionVisitor<T>(func);

            visitor.Visit(value);

            return visitor.Context;
        }

        public static IEnumerable<FilterModel> ToCollection<T>(this Expression<Func<T, bool>> value,
            Action<string, string> func = null)
            where T : IEntity
        {
            IList<FilterModel> result = new List<FilterModel>();
           
            value.CreateObject<T>((mem, data) =>
            {
                result.Add(new FilterModel { Field = mem, Value = data });
            });

            return result;
        }
   
        /// <summary>
        /// To Transfer some expression node type T to V.
        /// </summary>
        /// <param name="filter">Input filter expression.</param>
        /// <returns>Result of V type filter expression.</returns>
        public static Expression<Func<V, bool>> To<T, V>(this Expression<Func<T, bool>> filter)
        {
            try
            {
                var t = typeof(T).GetProperties().Select(x => x.Name).ToList();
                var v = typeof(V).GetProperties().Select(x => x.Name).ToList();
                var s = t.Except(v).ToList();
                var another = (Expression<Func<T, bool>>)new EraseExperssionVisitor(s.ToArray())
                    .Visit(filter);

                //transform into simple expression to suit with Dynamic Expression.
                var e = new SimpleExpressionVisitor();
                e.Visit(another);

                var b = ReplaceTrueString(e.SimpleContext.ToString());

                return System.Linq.Dynamic.DynamicExpression.ParseLambda<V, bool>(b, new[]
                {
                    Expression.Parameter(typeof(V),
                    typeof(V).Name)
                });
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// To erase some expression node.
        /// </summary>
        /// <param name="filter">Input expression.</param>
        /// <param name="members">Array of erase members name.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Erase<T>(this Expression<Func<T, bool>> filter,
            params string[] members)
        {
            try
            {
                var another = (Expression<Func<T, bool>>)new EraseExperssionVisitor(members)
                    .Visit(filter);

                //transform into simple expression to suit with Dynamic Expression.
                var e = new SimpleExpressionVisitor();
                e.Visit(another);

                var b = ReplaceTrueString(e.SimpleContext.ToString());

                return System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(b, new[]
                {
                    Expression.Parameter(typeof(T),
                    typeof(T).Name)
                });
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Replace some string such as True with expression understandable string value.
        /// </summary>
        /// <param name="str">Input filter string.</param>
        /// <returns></returns>
        private static string ReplaceTrueString(string str)
        {
            if (str == string.Empty)
                return "1 == 1";

            return str
                .Replace("(True && True)", "(1 == 1)")
                    .Replace("(True || True)", "(1 == 1)")
                    .Replace("&& True", "&& 1 == 1")
                    .Replace("|| True", "|| 1 == 1")
                    .Replace("(True &&", "( 1 == 1 &&")
                    .Replace("(True ||", "( 1 == 1 &&")
                    .Trim();
        }

        public class FilterExperssionVisitor<T> : ExpressionVisitor
            where T : IEntity
        {
            private string _currentMemberName;
            private Action<string, string> _func;

            public T Context { get; private set; }

            public FilterExperssionVisitor(Action<string, string> func)
            {
                Context = Activator.CreateInstance<T>();
                _func = func;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(string)
                    && (node.Method.Name == "Equals" ||
                    node.Method.Name == "Contains" ||
                    node.Method.Name == "StartsWith" ||
                    node.Method.Name == "EndsWith"))
                {
                    Visit(Expression.Equal(node.Object, node.Arguments[0]));

                    return node;
                }
                else if (!string.IsNullOrEmpty(node.Method.Name))
                {
                    var getter = node.CreateDynamic(typeof(object));
                    Visit(Expression.Constant(getter));
                    return node;
                }

                throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                Visit(node.Left);
                Visit(node.Right);

                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                var value = node.Value;

                if (this._currentMemberName != null)
                {
                    if (_func != null)
                        _func(this._currentMemberName, value.ToString());

                    this.Context
                        .GetType()
                        .GetProperty(this._currentMemberName)
                        .SetValue(this.Context, value);
                }
                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
                {
                    _currentMemberName = node.Member.Name;
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

    public class FilterModel
    {
        public string Field { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// Erase or replace search expression with true value.
    /// </summary>
    class EraseExperssionVisitor : ExpressionVisitor
    {
        private string[] _member = new string[] { };
        public EraseExperssionVisitor(string[] member)
        {
            _member = member;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal
                || node.NodeType == ExpressionType.NotEqual
                || node.NodeType == ExpressionType.LessThan
                || node.NodeType == ExpressionType.LessThanOrEqual
                || node.NodeType == ExpressionType.GreaterThan
                || node.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                foreach (var child in new[] { node.Left, node.Right })
                {
                    var memberEx = child as MemberExpression;
                    if (memberEx != null && _member.Contains(memberEx.Member.Name))
                    {
                        if (_member.Where(x => x == memberEx.Member.Name).Count() > 0)
                        {
                            return Expression.Constant(true);
                        }
                    }
                }
            }

            return base.VisitBinary(node);
        }
    }

    /// <summary>
    /// Transformation of simple expression like a == "Foo" && b == "Bar"
    /// </summary>
    class SimpleExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Expression of string result.
        /// </summary>
        public StringBuilder SimpleContext { get; private set; } = new StringBuilder();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var getter = node.CreateDynamic(typeof(object));
            Visit(Expression.Constant(getter));
            return node;

            throw new NotSupportedException($"The method '{node.Method.Name}' is not supported.");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SimpleContext.Append("(");

            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    SimpleContext.Append(" && ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    SimpleContext.Append(" || ");
                    break;

                case ExpressionType.Equal:
                    SimpleContext.Append(" == ");
                    break;

                case ExpressionType.NotEqual:
                    SimpleContext.Append(" != ");
                    break;

                case ExpressionType.LessThan:
                    SimpleContext.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    SimpleContext.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    SimpleContext.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    SimpleContext.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
            }

            Visit(node.Right);

            SimpleContext.Append(")");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is string)
                SimpleContext.Append("\"" + node.Value + "\"");
            else
                SimpleContext.Append(node.Value);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {

            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SimpleContext.Append(node.Member.Name);
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
