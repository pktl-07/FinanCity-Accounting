namespace ESS.Web.Data.Extensions
{
    using System;
    using System.Linq;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.DependencyInjection;
    using Infrastructure;
    using Infrastructure.Implementation;

    internal static class CollectionExtensions
    {
        /// <summary>
        /// Replace null value with custom data before db commiting.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="replacevalue"></param>
        /// <returns></returns>
        //internal static IList<DbParameter> RemoveNullValue(this List<DbParameter> collection)
        //{
        //    for (int i = 0; i < collection.Count; i++)
        //        if (collection[i].Value.ToString() == GeneralUtility.NullConstant)
        //            collection.Remove(collection[i]);

        //    return collection;
        //}

        /// <summary>
        /// Ensure to transform upper casing format depends on configuration.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static IList<DbParameter> ConvertToLetterCaseOnDemand(this IList<DbParameter> collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (DI.Current.BuildServiceProvider().GetService<IEssAppRepository>().LetterFormat == LetterCasing.Upper)
                {
                    if (collection[i].Value is string)
                        collection[i].Value = collection[i].Value.ToString().ToUpper().Trim();
                }
                else if (DI.Current.BuildServiceProvider().GetService<IEssAppRepository>().LetterFormat == LetterCasing.Lower)
                {
                    if (collection[i].Value is string)
                        collection[i].Value = collection[i].Value.ToString().ToLower().Trim();
                }
            }
            return collection;
        }

        internal static ICollection<string> InsertValidate(this ICollection<string> source)
        {
            FoundThenReplace(ref source, typeof(IKey), GeneralUtility.GetKey);
            //FoundThenReplace(ref source, typeof(ICreatedUser), DI.Current.BuildServiceProvider().GetService<IEssDataRepository>().LoginName);
            FoundThenReplace(ref source, typeof(ICreatedDate), GeneralUtility.ServerDate);
            FoundThenReplace(ref source, typeof(ICreaetedTime), GeneralUtility.ServerTime);
            FoundThenReplace(ref source, typeof(IRecordStatus), (int)RecordStatus.Save);
            FoundThenReplace(ref source, typeof(IModifiedDate),GeneralUtility.ServerDate);
            FoundThenRemove(ref source, typeof(IAuto));
            FoundThenRemove(ref source, typeof(IModifiedUser));
            //FoundThenRemove(ref source, typeof(IModifiedDate));
            FoundThenRemove(ref source, typeof(IModifiedTime));
            FoundThenRemove(ref source, typeof(IRecursiveArray));
            //RemoveNull(ref source);
            return source;
        }

        internal static ICollection<string> UpdateValidate(this ICollection<string> source)
        {
            FoundThenRemove(ref source, typeof(IKey));
            FoundThenRemove(ref source, typeof(IAuto));
            //FoundThenRemove(ref source, typeof(ICreatedUser));
            FoundThenRemove(ref source, typeof(ICreaetedTime));
            FoundThenReplace(ref source, typeof(IModifiedUser), DI.Current.BuildServiceProvider().GetService<IEssDataRepository>().LoginName);
            FoundThenReplace(ref source, typeof(IModifiedDate), GeneralUtility.ServerDate);
            FoundThenReplace(ref source, typeof(IModifiedTime), GeneralUtility.ServerTime);
            FoundThenReplace(ref source, typeof(IRecordStatus), (int)RecordStatus.Update);
            FoundThenRemove(ref source, typeof(IRecursiveArray));
            //RemoveNull(ref source);
            return source;
        }

        internal static IDictionary<string, object> ToDictionary(this DbParameterCollection param)
        {
            Dictionary<string, object> target = new Dictionary<string, object>();
            
            for (int i = 0; i < param.Count; i++)
            {
                target.Add(param[i].ParameterName, param[i].Value);
            }
          
            return target;
        }

        private static void FoundThenReplace<T>(ref ICollection<string> source, Type type, T replaceValue)
        {
            var isource = type.GetProperties().Select(x => x.Name);
            var keyPair = source.Select(x => new { Key = x.Split('=')[0].Trim(), Value = SQLParamRegEx(x.Split('=')[1].Trim()) }).ToList();
            var intercept = isource.Intersect(keyPair.Select(x => x.Key));
            var except = isource.Except(keyPair.Select(x => x.Key));

            if (intercept.Any())
            {
                foreach (var key in intercept)
                {
                    source.Remove(source.Where(x => x.Split('=')[0] == key).Single());
                    var originValue = keyPair.Where(x => x.Key == key).Single().Value;
                    
                    //replace the syskey value when it's equal to null
                    //else assingn original value.
                    if (type.Name.Equals("IKey", StringComparison.OrdinalIgnoreCase))
                    {
                        if (originValue == GeneralUtility.NullConstant)
                            source.Add($"{key}={DbParameterAnnotation.ConverTo(replaceValue)}");
                        else
                            source.Add($"{key}={DbParameterAnnotation.ConverTo(originValue)}");
                    }
                    //replace the record status value when it's unassign integer value.
                    //else assign original value.
                    else if (type.Name.Equals("IRecordStatus", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.Parse(originValue) == (int)RecordStatus.New)
                            source.Add($"{key}={DbParameterAnnotation.ConverTo(replaceValue)}");
                        else
                            source.Add($"{key}={DbParameterAnnotation.ConverTo(originValue)}");
                    }
                    else
                    {
                        source.Add($"{key}={DbParameterAnnotation.ConverTo(replaceValue)}");
                    }
                }
            }
            if (except.Any())
            {
                foreach (var key in except)
                {
                    source.Add($"{key}={DbParameterAnnotation.ConverTo(replaceValue)}");
                }
            }
        }

        private static void FoundThenRemove(ref ICollection<string> source, Type type)
        {
            var isource = type.GetProperties().Select(x => x.Name);
            var keyPair = source.Select(x => new { Key = x.Split('=')[0].Trim(), Value = SQLParamRegEx(x.Split('=')[1].Trim()) }).ToList();
            var intercept = isource.Intersect(keyPair.Select(x => x.Key));

            if (intercept.Any())
                foreach (var key in intercept)
                    source.Remove(source.Where(x => x.Split('=')[0] == key).Single());
        }

        private static string SQLParamRegEx(string param)
        {
            var m = Regex.Match(param, GeneralUtility.ParamRegEx);

            if (m.Success)
                return m.Groups["param"].ToString().Trim();
            else
                return null;
        }

        //private static void RemoveNull(ref ICollection<string> source)
        //{
        //    var keyPair = source.Select(x => new { Key = x.Split('=')[0].Trim(), Value = SQLParamRegEx(x.Split('=')[1].Trim()) }).ToList();

        //    foreach (var val in keyPair)
        //        if (val.Value == GeneralUtility.NullConstant)
        //            source.Remove(source.Where(x => x.Split('=')[0] == val.Key).Single());
        //}
    }
}
