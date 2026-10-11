namespace FC.Core.Infrastructure.ConfigurationManager
{
    using System.Web;
    using System.Configuration;
    using FC.Core;

    public class GeneralConfigSection : ConfigurationSection
    {
      
        [ConfigurationProperty("currentUtc", IsRequired = true)]
        public int CurrentUtc
        {
            get
            {
                return (int)this["currentUtc"];
            }
        }

        [ConfigurationProperty("dateFormat", IsRequired = true)]
        public string DateFormat
        {
            get
            {
                return (string)this["dateFormat"];
            }
        }

        [ConfigurationProperty("letterFormat", IsRequired = true)]
        public LetterCasing LetterFormat
        {
            get
            {
                return ((LetterCasing)(int)this["letterFormat"]);
            }
        }

        [ConfigurationProperty("currsk", IsRequired = true)]
        public string CURRSK
        {
            get
            {
                return (string)this["currsk"];
            }
        }

        [ConfigurationProperty("currate", IsRequired = true)]
        public decimal CURRATE
        {
            get
            {
                return (decimal)this["currate"];
            }
        }

        [ConfigurationProperty("currop", IsRequired = true)]
        public string CURROP
        {
            get
            {
                return (string)this["currop"];
            }
        }

        [ConfigurationProperty("currcode", IsRequired = true)]
        public string CURRCODE
        {
            get
            {
                return (string)this["currcode"];
            }
        }

        [ConfigurationProperty("domainUrl", IsRequired = true)]
        public string DomainUrl
        {
            get
            {
                return (string)this["domainUrl"];
            }
        }

        [ConfigurationProperty("frontEndUrl", IsRequired = true, DefaultValue = null)]
        public string FrontEndUrl
        {
            get
            {
                return (string)this["frontEndUrl"];
            }
        }

        [ConfigurationProperty("defaultCountry", IsRequired = true, DefaultValue = null)]
        public string DefaultCountry
        {
            get
            {
                return (string)this["defaultCountry"];
            }
        }

        [ConfigurationProperty("companyName", IsRequired = true, DefaultValue = null)]
        public string CompanyName
        {
            get
            {
                return (string)this["companyName"];
            }
        }

        [ConfigurationProperty("reportTemplateDirectory", IsRequired = true, DefaultValue = null)]
        public string ReportTemplateDirectory
        {
            get
            {
                return (string)this["reportTemplateDirectory"];
            }
        }

        [ConfigurationProperty("sharedReportTemplateDirectory", IsRequired = true, DefaultValue = null)]
        public string SharedReportTemplateDirectory
        {
            get
            {
                return (string)this["sharedReportTemplateDirectory"];
            }
        }

        public string SPSK
        {
            get;
            set;
        }

        [ConfigurationProperty("removeLeadWhoNeverBecomeClientForDays", IsRequired = false, DefaultValue = 60)]
        public int RemoveLeadWhoNeverBecomeClientForDays
        {
            get
            {
                return (int)this["removeLeadWhoNeverBecomeClientForDays"];
            }
        }

        [ConfigurationProperty("enableSalespersonTierBonus", IsRequired = false, DefaultValue = false)]
        public bool EnableSalespersonTierBonus
        {
            get
            {
                return (bool)this["enableSalespersonTierBonus"];
            }
        }
    }
}
