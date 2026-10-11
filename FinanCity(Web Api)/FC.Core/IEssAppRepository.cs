namespace FC.Core
{
    using System.Collections.Generic;

    public interface IEssAppRepository : IEssRepository, IHideObjectMembers
    {
        ///<summary>
        /// Get server utc format eg. 8 for Singapore. 
        ///</summary>
        ///<remarks>
        ///The change will never be effected after one time cofiguration made.
        ///</remarks>
        int CurrentUtc { get; }

        /// <summary>
        /// Get date format eg.dd/MM/yyyy
        /// </summary>
        ///<remarks>
        ///The change will never be effected after one time cofiguration made.
        ///</remarks>
        string DateFormat { get; }

        /// <summary>
        /// Covert to all custom letter casing format on db dubmit.
        /// </summary>
        ///<remarks>
        ///The change will never be effected after one time cofiguration made.
        ///</remarks>
        LetterCasing LetterFormat { get; }

        /// <summary>
        /// Base Api Url.
        /// </summary>
        //string BaseApiUrlName { get; }

        /// <summary>
        /// ApplicationType
        /// </summary>
        //int ApplicationType { get; }

        /// <summary>
        /// Currency SysKey
        /// </summary>
        string CURRSK { get; }

        /// <summary>
        /// Currency Rate
        /// </summary>
        decimal CURRATE { get; }

        /// <summary>
        /// Currency operator example "%" or "*"
        /// </summary>
        string CURROP { get; }

        /// <summary>
        /// Currency code example "%" or "*"
        /// </summary>
        string CURRCODE { get; }

        /// <summary>
        /// Salesperson SysKey
        /// </summary>
        string SPSK { get; set; }

        /// <summary>
        /// Resource url example "https://lir.peas.sg"
        /// </summary>
        // string ResourceUrl { get; }

        /// <summary>
        /// Resource url example "https://lir.peas.sg"
        /// </summary>
        string DomainUrl { get; }

        /// <summary>
        /// Resource url example "https://financity.peas.sg"
        /// </summary>
        string FrontEndUrl { get; }

        /// <summary>
        /// Default Country to set for Property transaction and client it's contact.
        /// </summary>
        string DefaultCountry { get; }

        /// <summary>
        /// Default Company Name.
        /// </summary>
        string CompanyName { get; }

        /// <summary>
        /// Reports Template Directory.
        /// </summary>
        string ReportTemplateDirectory { get; }

        /// <summary>
        /// Shared Reports Template Directory.
        /// </summary>
        string SharedReportTemplateDirectory { get;  }

        /// <summary>
        /// Enable Salesperson promote to next level.
        /// </summary>
        //bool EnableSalespersonPromotion { get; set; }

        /// <summary>
        /// Enable Salesperson tier bonus for his tier 1,2 ect.. according to their commission structure.
        /// </summary>
        bool EnableSalespersonTierBonus { get;  }

        /// <summary>
        /// Default account code.
        /// </summary>
        DefaultAccountingCode DefaultAccountingCode { get;  }

        /// <summary>
        /// Default sales tax.
        /// </summary>
        DefaultCompanyTax DefaultSalesTax { get;  }

        /// <summary>
        /// Default purchase tax.
        /// </summary>
        DefaultCompanyTax DefaultPurchaseTax { get;  }

        /// <summary>
        /// Default zero purchase tax.
        /// </summary>
        DefaultCompanyTax DefaultPurchaseZeroTax { get;  }

        /// <summary>
        /// Current Api Version No.
        /// </summary>
        string AppVersionInfo { get; }
    }

    public class DefaultCompanyTax
    {
        public string TaxSK { get; set; }
        public string TaxCode { get; set; }
        public decimal TaxPercent { get; set; }
    }

    public class DefaultAccountingCode
    {
        public KeyValuePair<string, string> Cash { get; set; }

        public KeyValuePair<string, string> CommisionReceivable { get; set; }

        public KeyValuePair<string, string> InputTax { get; set; }

        public KeyValuePair<string, string> OutputTax { get; set; }

        public KeyValuePair<string, string> SalespersonAccurals { get; set; }

        public KeyValuePair<string, string> MarketingPromotionExpense { get; set; }

        public KeyValuePair<string, string> MarketingPromotionPayable { get; set; }

        public KeyValuePair<string, string> CobrokerAccurals { get; set; }
        
        public KeyValuePair<string, string> RetaninEarning { get; set; }
        
        public KeyValuePair<string, string> CompanyCapital { get; set; }

        public KeyValuePair<string, string> CommisionExpense { get; set; }

        public KeyValuePair<string, string> PromotionalExpense { get; set; }

        public KeyValuePair<string, string> ExchangeGain { get; set; }

        public KeyValuePair<string, string> ExchangeLoss { get; set; }

        public KeyValuePair<string, string> DecimalAdjustment { get; set; }
    }
}

