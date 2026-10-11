namespace FC.Api.Infrastructure
{
    using System.Timers;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using Ninject;
    using FC.Core;
    using FC.Core.Extensions;
    using FC.Domain.Sales.Business;
    using FC.Core.Infrastructure.ConfigurationManager;
    using FC.Core.SignalR;
   
    /// <summary>
    /// FinanCity globle setting.
    /// </summary>
    public class FinanCityGlobleSetting
    {
        /// <summary>
        /// Register all configurations in one place.
        /// </summary>
        /// <returns>Task</returns>
        public async static Task AsyncRegisterAllConfiguration()
        {
            await Task.Run(async () =>
            {
                await AsyncRegisterConfiguration();
                NotifyOnVersionChange();
            });

            var timer = new Timer(24 * 60 * 60 * 1000);
            timer.Elapsed += new ElapsedEventHandler((s, e) =>
            {
                FCHub.NotifyOnLeadFollowUp();

                var _lead = DI.Current.Get<Lead>();
                var config = DI.Current.Get<FinanCityConfig>();

                _lead.InActiveLeadWhoNeverBecomeClientFor(day: config.GeneralConfiguration.RemoveLeadWhoNeverBecomeClientForDays);
            });

            timer.Enabled = true;
        }

        /// <summary>
        /// Async register for globle configuraton such as database connection
        /// and required dependencies.
        /// </summary>
        /// <returns>Task</returns>
        public async static Task AsyncRegisterConfiguration()
        {
            await Task.Run(() =>
            {
                var kernel = DI.Current.Get<IKernel>();
                var config = DI.Current.Get<FinanCityConfig>();
                var defaultAccount = config.DefaultAccountConfiguration;

                kernel.UseEarthStone(appservice =>
                {
                    appservice.DomainUrl = config.GeneralConfiguration.DomainUrl;
                    appservice.CurrentUtc = config.GeneralConfiguration.CurrentUtc;
                    appservice.FrontEndUrl = config.GeneralConfiguration.FrontEndUrl;
                    appservice.DefaultCountry = config.GeneralConfiguration.DefaultCountry;
                    appservice.DateFormat = config.GeneralConfiguration.DateFormat;
                    appservice.LetterFormat = config.GeneralConfiguration.LetterFormat;
                    appservice.CURRATE = config.GeneralConfiguration.CURRATE;
                    appservice.CURROP = config.GeneralConfiguration.CURROP;
                    appservice.CURRSK = config.GeneralConfiguration.CURRSK;
                    appservice.CURRCODE = config.GeneralConfiguration.CURRCODE;
                    appservice.CompanyName = config.GeneralConfiguration.CompanyName;
                    appservice.RemoveLeadWhoNeverClientForDays = config.GeneralConfiguration.RemoveLeadWhoNeverBecomeClientForDays;
                    appservice.ReportTemplateDirectory = config.GeneralConfiguration.ReportTemplateDirectory;
                    appservice.SharedReportTemplateDirectory = config.GeneralConfiguration.SharedReportTemplateDirectory;
                    appservice.DefaultSalesTax = new DefaultCompanyTax { TaxCode = "SR", TaxPercent = 7, TaxSK = "201307051139513774165001" };
                    appservice.DefaultPurchaseTax = new DefaultCompanyTax { TaxCode = "TX7", TaxPercent = 7, TaxSK = "201307051129191477965001" };
                    appservice.DefaultPurchaseZeroTax = new DefaultCompanyTax { TaxCode = "T0", TaxPercent = 0, TaxSK = "1" };
                    appservice.AppVersionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    appservice.DefaultAccountingCode = new DefaultAccountingCode
                    {
                        Cash = new KeyValuePair<string, string>(defaultAccount.Cash.AccountSK, defaultAccount.Cash.Description),
                        CommisionReceivable = new KeyValuePair<string, string>(defaultAccount.CommisionReceivable.AccountSK, defaultAccount.CommisionReceivable.Description),
                        InputTax = new KeyValuePair<string, string>(defaultAccount.InputTax.AccountSK, defaultAccount.InputTax.Description),
                        MarketingPromotionExpense = new KeyValuePair<string, string>(defaultAccount.MarketingPromotionExpense.AccountSK, defaultAccount.MarketingPromotionExpense.Description),
                        MarketingPromotionPayable = new KeyValuePair<string, string>(defaultAccount.MarketingPromotionPayable.AccountSK, defaultAccount.MarketingPromotionPayable.Description),
                        OutputTax = new KeyValuePair<string, string>(defaultAccount.OutputTax.AccountSK, defaultAccount.OutputTax.Description),
                        SalespersonAccurals = new KeyValuePair<string, string>(defaultAccount.SalespersonAccurals.AccountSK, defaultAccount.SalespersonAccurals.Description),
                        CobrokerAccurals = new KeyValuePair<string, string>(defaultAccount.CobrokerAccurals.AccountSK, defaultAccount.CobrokerAccurals.Description),
                        RetaninEarning = new KeyValuePair<string, string>(defaultAccount.RetaninEarning.AccountSK, defaultAccount.RetaninEarning.Description),
                        CompanyCapital = new KeyValuePair<string, string>(defaultAccount.CompanyCapital.AccountSK, defaultAccount.CompanyCapital.Description),
                        CommisionExpense = new KeyValuePair<string, string>(defaultAccount.CommisionExpense.AccountSK, defaultAccount.CommisionExpense.Description),
                        PromotionalExpense = new KeyValuePair<string, string>(defaultAccount.PromotionalExpense.AccountSK, defaultAccount.PromotionalExpense.Description),
                        ExchangeGain = new KeyValuePair<string, string>(defaultAccount.ExchangeGain.AccountSK, defaultAccount.ExchangeGain.Description),
                        ExchangeLoss = new KeyValuePair<string, string>(defaultAccount.ExchangeLoss.AccountSK, defaultAccount.ExchangeLoss.Description),
                        DecimalAdjustment = new KeyValuePair<string, string>(defaultAccount.DecimalAdjustment.AccountSK, defaultAccount.DecimalAdjustment.Description),
                    };
                }, dataservice =>
                {
                    dataservice.DbConnection = new Dictionary<string, DataConnection>
                    {
                        [CommonConstant.SalesConnection] = new DataConnection { ConnectionString = ConfigurationManager.ConnectionStrings["FC.Biz"].ConnectionString, Provider = DbProvider.MSSQL },
                        [CommonConstant.AccountConnection] = new DataConnection { ConnectionString = ConfigurationManager.ConnectionStrings["FC.Acc"].ConnectionString, Provider = DbProvider.MSSQL },
                        [CommonConstant.AuthConnection] = new DataConnection { ConnectionString = ConfigurationManager.ConnectionStrings["FC.Auth"].ConnectionString, Provider = DbProvider.MSSQL },
                    };
                });
            });
        }

        /// <summary>
        /// Async for remarks status on db insert event.
        /// </summary>
        /// <returns></returns>
        public static void NotifyOnVersionChange()
        {
            VersionFileWatcher.Changed += (s, e) =>
            {
                FCHub.NotifyOnAndriodVersionChange();
                FCHub.NotifyOnIOSVersionChange();
            };
        }
    }
}