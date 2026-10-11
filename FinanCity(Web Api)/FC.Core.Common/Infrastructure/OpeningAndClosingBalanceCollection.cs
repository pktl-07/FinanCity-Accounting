namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    public class OpeningAndClosingBalanceCollection<T> where T : new()
    {
        #region Fields

        private List<T> source;

        private string openingFieldName, closingFieldName, balanceFieldName;

        private List<string> toLookup = new List<string>();

        #endregion

        #region Method

        public OpeningAndClosingBalanceCollection<T> Intialize(List<T> source, Expression<Func<T, object>> opentingField, Expression<Func<T, object>> runningField, Expression<Func<T, object>> balanceField)
        {
            this.source = source;
            this.openingFieldName = new ExpressionHelper<T>().GetFieldName(opentingField);
            this.closingFieldName = new ExpressionHelper<T>().GetFieldName(runningField);
            this.balanceFieldName = new ExpressionHelper<T>().GetFieldName(balanceField);

            return this;
        }

        /// <summary>
        /// To Look Up The Previous Row Field Value
        /// </summary>
        /// <param name="expression">Field Expression</param>
        /// <returns></returns>
        public OpeningAndClosingBalanceCollection<T> ToLookUp(Expression<Func<T, object>> expression)
        {
            toLookup.Add(new ExpressionHelper<T>().GetFieldName(expression));

            return this;
        }

        /// <summary>
        /// To Get Result Of Opening And Closing Balance
        /// </summary>
        /// <returns></returns>
        public List<T> GetResult()
        {

            for (int i = 0; i < source.Count(); i++)
            {
                var local = source[i].GetType();

                decimal openingAmount = (decimal)local.GetProperty(openingFieldName).GetValue(source[i]);

                decimal closingAmount = (decimal)local.GetProperty(closingFieldName).GetValue(source[i]);

                if (i == 0) // fatch first record
                {
                    SetRowRecord(source[i], openingAmount, closingAmount, openingAmount - closingAmount);
                }
                else
                {
                    bool flag = true;

                    foreach (var v in toLookup)
                    {
                        if ((string)local.GetProperty(v).GetValue(source[i]) == (string)local.GetProperty(v).GetValue(source[i - 1]))
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        openingAmount = (decimal)local.GetProperty(balanceFieldName).GetValue(source[i - 1]); // redefine for opening amount

                        SetRowRecord(source[i], openingAmount, closingAmount, openingAmount - closingAmount);
                    }
                    else
                    {
                        SetRowRecord(source[i], openingAmount, closingAmount, openingAmount - closingAmount);
                    }
                }
            }

            return source.ToList();
        }

        #endregion

        #region Util

        private void SetRowRecord(T t, decimal openingAmt, decimal closingAmt, decimal balanceAmt)
        {
            t.GetType().GetProperty(openingFieldName).SetValue(t, openingAmt);

            t.GetType().GetProperty(closingFieldName).SetValue(t, closingAmt);

            t.GetType().GetProperty(balanceFieldName).SetValue(t, openingAmt - closingAmt);
        }
        #endregion
    }

    public class ExpressionHelper<T>
    {
        private MemberExpression GetMemberExpression(Expression<Func<T, object>> type)
        {
            MemberExpression logicalExpression;
            switch (type.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    // intialize unary expression which is non string type
                    var logicalUnaryExpression = type.Body as UnaryExpression;
                    logicalExpression = ((logicalUnaryExpression != null) ? logicalUnaryExpression.Operand : null) as MemberExpression;
                    break;
                default:
                    // intialize expression which is string type 
                    logicalExpression = type.Body as MemberExpression;
                    break;
            }

            return logicalExpression;
        }

        public virtual string GetFieldName(Expression<Func<T, object>> physicalName)
        {
            return GetMemberExpression(physicalName).Member.Name;
        }

        public A GetMemberAttribute<A>(Expression<Func<T, object>> physicalName) where A : Attribute
        {
            A target = null;

            object[] attrs;

            attrs = GetMemberExpression(physicalName).Member.GetCustomAttributes(typeof(A), false);

            foreach (var attr in attrs)
                target = (A)attr;


            return target;
        }

        public A GetClassAttribute<A>(Type type) where A : Attribute
        {
            A target = null;

            var attrs = type.GetCustomAttributes(typeof(A), false);

            foreach (var att in attrs)
            {
                target = (A)att;
            }

            return target;
        }
    }
}
