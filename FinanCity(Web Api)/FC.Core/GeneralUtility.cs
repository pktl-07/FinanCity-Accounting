namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Ninject;

    public partial class GeneralUtility
    {
        public const string NullConstant = "DBREPLACENULLVALUE";
        public const string ParamRegEx = @"<(?'tag'\#).(?'param'.*?)\k'tag'>";
        public const string CommaRegEx = @"""[^""\r\n]*""|'[^'\r\n]*'|[^,\r\n]*";
        public const string DMYRegEx = @"^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d$";

        public static string ConvertDisplayDateFormat(string date)
        {
            try
            {
                if (date.Length == 0)
                {
                    return "";
                }

                var reg = new Regex(DMYRegEx);
                var match = reg.Match(date);
                if (match.Success)
                {
                    return date;
                }
                else
                {
                    var year = date.Substring(0, 4);
                    var month = date.Substring(4, 2);
                    var day = date.Substring(6, 2);

                    return $"{day}/{month}/{year}";
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ConvertDisplayDateFormat(int? date)
        {
            try
            {
                if (date != null)
                    return ConvertDisplayDateFormat(date.Value.ToString());
                
                return null;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public static string ConvertDisplayDateFormat(int date)
        {
            try
            {
                return ConvertDisplayDateFormat(date.ToString());
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public static DateTime? ConvertDateFormat(string dateString)
        {
            try
            {
                if (string.IsNullOrEmpty(dateString))
                {
                    return null;
                }

                var year = Convert.ToInt32(dateString.Substring(0, 4));
                var month = Convert.ToInt32(dateString.Substring(4, 2));
                var day = Convert.ToInt32(dateString.Substring(6, 2));
                var date = new DateTime(year, month, day);

                return date.Date;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ConvertDateFormat(int? date)
        {
            try
            {
                if (date == null)
                {
                    return null;
                }

                var year = Convert.ToInt32(date.ToString().Substring(0, 4));
                var month = Convert.ToInt32(date.ToString().Substring(4, 2));
                var day = Convert.ToInt32(date.ToString().Substring(6, 2));
                var d = new DateTime(year, month, day);

                return d.Date;
            }
            catch
            {
                return null;
            }
        }

        public static string ConvertSystemDateStringFormat(string date)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(date))
                {
                    if (date.Length == 8)
                    {
                        int i = 0;

                        if (int.TryParse(date, out i))
                            return date;
                    }
                    if (date.Length != 10)
                        throw new System.Exception("Date format is incorrect");

                    if (date.Contains('/'))
                    {
                        var strDate = date.Split('/');
                        var tryPass1 = int.Parse(strDate[2]);
                        var tryPass2 = int.Parse(strDate[0]);
                        var tryPass3 = int.Parse(strDate[1]);
                        return $"{strDate[2]}{strDate[1]}{strDate[0]}";
                    }
                    else
                    {
                        return date;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string ConvertSystemTimeFormat(DateTime date)
        {
            return $"{date.Hour.ToString("00")}{date.Minute.ToString("00")}{date.Second.ToString("00")}";
        }

        public static string ConvertSystemDateStringFormat(DateTime? date)
        {
            try
            {
                if (date.HasValue)
                {
                    var year = date.Value.Year;
                    var month = date.Value.Month;
                    var day = date.Value.Day;

                    var strYear = year.ToString("0000");
                    var strMonth = month.ToString("00");
                    var strDay = day.ToString("00");
                    return strYear + strMonth + strDay;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Key
        {
            get
            {
                //make sure millisecond has been changed. 
                Task.Delay(1).Wait();
                var currentTime = DateTime.UtcNow.AddHours(DI.Current.Get<IEssAppRepository>().CurrentUtc);
                currentTime.AddTicks(100);

                return GeneralUtility.ConvertSystemDateStringFormat(currentTime)
                   + GeneralUtility.ConvertSystemTimeFormat(currentTime)
                   + currentTime.Millisecond.ToString("0000");
            }
        }

        public static int ServerDate
        {
            get { return int.Parse(ConvertSystemDateStringFormat(DateTime.UtcNow.AddHours(DI.Current.Get<IEssAppRepository>().CurrentUtc))); }
        }

        public static int ServerYear
        {
            get { return int.Parse(ServerDate.ToString().Substring(0, 4)); }
        }

        public static string ServerTime
        {
            get { return ConvertSystemTimeFormat(DateTime.UtcNow.AddHours(DI.Current.Get<IEssAppRepository>().CurrentUtc)); }
        }

        public static class Message
        {
            public static string SubmissionComplete { get { return Resources.NotificationResource.SubmissionComplete; } }

            public static string SubmissionFail { get { return Resources.NotificationResource.SubmissionFail; } }

            public static string SaveComplete { get { return Resources.NotificationResource.SaveComplete; } }

            public static string UpdateComplete { get { return Resources.NotificationResource.UpdateComplete; } }

            public static string DeleteComplete { get { return Resources.NotificationResource.DeleteComplete; } }
        }
    }
}
