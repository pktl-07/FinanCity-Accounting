namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using Ninject;
    using FC.Domain.Report;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;

    public static class ExpressionExtensions
    {
        /// <summary>
        /// Produce formula field value and name base on filter expression
        /// especially to use it for Crystal Report formula fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">Input filter.</param>
        /// <returns>Formula fields dictionary.</returns>
        public static IDictionary<string, string> FormulaFieldFor<T>(this Expression<Func<T, bool>> filter)
            where T : IEntity
        {
            try
            {
                var dic = new Dictionary<string, string>();
                var formula = DI.Current.Get<IRegisterFormulaFieldFactory>();

                var type = filter.CreateObject((mem, value) =>
                {
                    var factory = formula.GetFactory(mem);

                    if (dic.Where(x => x.Key == mem).Count() == 0)
                    {
                        if (mem != null)
                            dic.Add(mem, factory.WriteFormat(value));
                    }
                    else
                        dic[mem] = factory.WriteFormat(value);
                });

                typeof(T).GetProperties().Select(x => x.Name)
                    .Except(dic.Keys).Each(mem =>
                    {
                        var factory = formula.GetFactory(mem);
                        dic.Add(mem, factory.WriteFormat(null));
                    });

                return dic;
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
    }
}
