namespace FC.Core 
{
    using System;
    using System.Linq.Expressions;
    using Newtonsoft.Json;
   
    /// <summary>
    /// Kendo Grid Filter Transalator.
    /// </summary>
    public class KendoGridFilterTransalator
    {
        public KendoGridFilterTransalator()
        {

        }

        #region Methods

        /// <summary>
        /// To convert expression from Kendo Grid incoming json filter data to our SQL understanable expression.
        /// </summary>
        /// <typeparam name="T">Type of entity or datatable.</typeparam>
        /// <param name="source">Kendo Grid filter expression.</param>
        /// <returns>SQL understandable input string.</returns>
        public Expression<Func<T, bool>> Convert<T>(string source)
        {
            if (source == null)
                return CreateExpression<T>("1 == 1");
        
            var result = "";
            var j = new KendoGridFilterModel();

            try
            {
                j = JsonConvert.DeserializeObject<KendoGridFilterModel>(source);
            }
            catch
            {
                return CreateExpression<T>(source.Replace("\"", "").Replace("'", "\""));
            }

            for (int i = 0; i < j.Filters.Length; i++)
            {
                for (int k = 0; k < j.Filters[i].Filters.Length; k++)
                {
                    var tinfo = j.Filters[i].Filters[k];

                    if (tinfo.Operator == "contains" || tinfo.Operator == "doesnotcontain"
                        || tinfo.Operator == "startswith" || tinfo.Operator == "endswith")

                        WriteRefTypeValue(tinfo: tinfo,
                            target: ref result,
                            type: typeof(T));
                    else

                        WriteStructTypeValue(tinfo: tinfo,
                            target: ref result,
                            type: typeof(T));

                    if (i < j.Filters.Length - 1)
                    {
                        switch (j.Filters[i].Logic)
                        {
                            case "and": result += " && "; break;
                            case "or": result += " || "; break;
                        }
                    }
                }
            }

            return string.IsNullOrWhiteSpace(result) ? CreateExpression<T>("1 == 1") :
                CreateExpression<T>(result.Replace("'", "\""));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// To Create Expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source of expression eg. SalesDate >= 20200101 && SalesDate <= 20201231 </param>
        /// <returns>Expression</returns>
        private Expression<Func<T, bool>> CreateExpression<T>(string source)
        {
            return System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(source.Trim(), new[]
            {
                Expression.Parameter(typeof(T),
                typeof(T).Name)
            });
        }

        /// <summary>
        /// To write on target input string when target value is start with expression eg.<# ( #>.
        /// </summary>
        /// <param name="target">input target</param>
        /// <returns>The target to compare</returns>
        private bool StartWithExpression(ref string target)
        {
            if (target != null && target.StartsWith("<#"))
            {
                target = target.Replace("<#", "")
                    .Replace("#>", "")
                    .Replace("(", "")
                    .Replace(")", "");

                return true;
            }
            return false;
        }

        /// <summary>
        /// To write on target input string when target value is end with expression eg.<# ) #>.
        /// </summary>
        /// <param name="target">input target</param>
        /// <returns>The target to compare</returns>
        private bool EndWithExpression(ref string target)
        {
            if (target != null && target.EndsWith("#>"))
            {
                target = target
                    .Replace("<#", "")
                    .Replace("#>", "")
                    .Replace("(", "")
                    .Replace(")", "");
        
                return true;
            }
            return false;
        }

        /// <summary>
        /// To convert CSharp operator (such as >,==)
        /// </summary>
        /// <param name="op">operator string defined by Kendo Grid json filter</param>
        /// <returns>Charp operator string</returns>
        private string ConvertCSharpOperator(string op)
        {
            if (string.IsNullOrWhiteSpace(op))
                throw new System.Exception("The operator can not be empty or null");

            if (op == "eq" || op == "isnull" || op == "isempty")
                return "==";
            if (op == "neq" || op == "isnotnull" || op == "isnotempty")
                return "!=";
            else if (op == "gt")
                return ">";
            else if (op == "gte")
                return ">=";
            else if (op == "lt")
                return "<";
            else if (op == "lte")
                return "<=";

            throw new System.Exception($@"{op} operator is not defined");
        }

        /// <summary>
        /// To Write on target input string for references type value.
        /// </summary>
        /// <param name="tinfo"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        private void WriteRefTypeValue(TransalatorFieldInfo tinfo, ref string target, Type type)
        {
            var info = tinfo.Value;
            if (tinfo.Operator == "contains")
            {
                if (StartWithExpression(ref info))
                    target += $@"({tinfo.Field}.Contains(""{info.Trim()}"")";
                else if (EndWithExpression(ref info))
                    target += $@"{tinfo.Field}.Contains(""{info.Trim()}""))";
                else
                    target += $@"{tinfo.Field}.Contains(""{info.Trim()}"")";
            }
            else if (tinfo.Operator == "doesnotcontain")
            {
                if (StartWithExpression(ref info))
                    target += $@"!({tinfo.Field}.Contains(""{info.Trim()}"")";
                else if (EndWithExpression(ref info))
                    target += $@"!{tinfo.Field}.Contains(""{info.Trim()}""))";
                else
                    target += $@"!{tinfo.Field}.Contains(""{info.Trim()}"")";
            }
           
            else if (tinfo.Operator == "startswith")
            {
                if (StartWithExpression(ref info))
                    target += $@"({tinfo.Field}.StartsWith(""{info.Trim()}"")";
                else if (EndWithExpression(ref info))
                    target += $@"{tinfo.Field}.StartsWith(""{info.Trim()}""))";
                else
                    target += $@"{tinfo.Field}.StartsWith(""{info.Trim()}"")";
            }
            else if (tinfo.Operator == "endswith")
            {
                if (StartWithExpression(ref info))
                    target += $@"({tinfo.Field}.EndsWith(""{info.Trim()}"")";
                else if (EndWithExpression(ref info))
                    target += $@"{tinfo.Field}.EndsWith(""{info.Trim()}""))";
                else
                    target += $@"{tinfo.Field}.EndsWith(""{info.Trim()}"")";
            }
        }

        /// <summary>
        /// To Write on target input string for structure value.
        /// </summary>
        /// <param name="target">Target string.</param>
        /// <param name="type">Type of entity or datatable.</param>
        /// <param name="op">operator string such as >,==.</param>
        /// <param name="field">Name of property of entity or datatable.</param>
        /// <param name="value">Value of property of entity or datatable.</param>
        private void WriteStructTypeValue(TransalatorFieldInfo tinfo, ref string target, Type type)
        {
            var startWithLogical = false;
            var endWithLogical = false;
            var value = tinfo.Value;

            if (value != null)
                value = value.Trim();

            if (StartWithExpression(ref value))
                startWithLogical = true;

            if (EndWithExpression(ref value))
                endWithLogical = true;

            if (startWithLogical)
                target += "(";

            var p = type.GetProperty(tinfo.Field).PropertyType;

            if (p == typeof(string))
                target += $@"{tinfo.Field} {ConvertCSharpOperator(tinfo.Operator)} ""{value}""";
            else
                target += $@"{tinfo.Field} {ConvertCSharpOperator(tinfo.Operator)} {value}";

            if (endWithLogical)
                target += ")";
        }

        #endregion
    }
}