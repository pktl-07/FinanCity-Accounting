namespace FC.Core.Infrastructure.ConfigurationManager
{
    using System.Configuration;
 
    public class DefaultAccountConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Cash")]
        public DefaultAccountElement Cash
        {
            get
            {
                return (DefaultAccountElement)this["Cash"];
            }
        }

        [ConfigurationProperty("CommisionReceivable")]
        public DefaultAccountElement CommisionReceivable
        {
            get
            {
                return (DefaultAccountElement)this["CommisionReceivable"];
            }
        }

        [ConfigurationProperty("InputTax")]
        public DefaultAccountElement InputTax
        {
            get
            {
                return (DefaultAccountElement)this["InputTax"];
            }
        }

        [ConfigurationProperty("MarketingPromotionExpense")]
        public DefaultAccountElement MarketingPromotionExpense
        {
            get
            {
                return (DefaultAccountElement)this["MarketingPromotionExpense"];
            }
        }

        [ConfigurationProperty("MarketingPromotionPayable")]
        public DefaultAccountElement MarketingPromotionPayable
        {
            get
            {
                return (DefaultAccountElement)this["MarketingPromotionPayable"];
            }
        }

        [ConfigurationProperty("OutputTax")]
        public DefaultAccountElement OutputTax
        {
            get
            {
                return (DefaultAccountElement)this["OutputTax"];
            }
        }

        [ConfigurationProperty("SalespersonAccurals")]
        public DefaultAccountElement SalespersonAccurals
        {
            get
            {
                return (DefaultAccountElement)this["SalespersonAccurals"];
            }
        }

        [ConfigurationProperty("CobrokerAccurals")]
        public DefaultAccountElement CobrokerAccurals
        {
            get
            {
                return (DefaultAccountElement)this["CobrokerAccurals"];
            }
        }

        [ConfigurationProperty("RetaninEarning")]
        public DefaultAccountElement RetaninEarning
        {
            get
            {
                return (DefaultAccountElement)this["RetaninEarning"];
            }
        }

        [ConfigurationProperty("CompanyCapital")]
        public DefaultAccountElement CompanyCapital
        {
            get
            {
                return (DefaultAccountElement)this["CompanyCapital"];
            }
        }

        [ConfigurationProperty("CommisionExpense")]
        public DefaultAccountElement CommisionExpense
        {
            get
            {
                return (DefaultAccountElement)this["CommisionExpense"];
            }
        }

        [ConfigurationProperty("PromotionalExpense")]
        public DefaultAccountElement PromotionalExpense
        {
            get
            {
                return (DefaultAccountElement)this["PromotionalExpense"];
            }
        }

        [ConfigurationProperty("ExchangeGain")]
        public DefaultAccountElement ExchangeGain
        {
            get
            {
                return (DefaultAccountElement)this["ExchangeGain"];
            }
        }

        [ConfigurationProperty("ExchangeLoss")]
        public DefaultAccountElement ExchangeLoss
        {
            get
            {
                return (DefaultAccountElement)this["ExchangeLoss"];
            }
        }

        [ConfigurationProperty("DecimalAdjustment")]
        public DefaultAccountElement DecimalAdjustment
        {
            get
            {
                return (DefaultAccountElement)this["DecimalAdjustment"];
            }
        }
    }
    public class DefaultAccountElement : ConfigurationElement
    {
        [ConfigurationProperty("AccountSK", IsRequired = true)]
        public string AccountSK
        {
            get
            {
                return (string)this["AccountSK"];
            }
        }

        [ConfigurationProperty("Description", IsRequired = true)]
        public string Description
        {
            get
            {
                return (string)this["Description"];
            }
        }
    }
}
