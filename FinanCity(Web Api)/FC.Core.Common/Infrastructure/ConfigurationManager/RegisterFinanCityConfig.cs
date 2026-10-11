namespace FC.Core.Infrastructure.ConfigurationManager
{
    using System;
    using System.Configuration;

    public class FinanCityConfig : ConfigurationSection
    {
        public GeneralConfigSection GeneralConfiguration
        {
            get
            {
                try
                {
                    return (GeneralConfigSection)ConfigurationManager.GetSection("FinanCity/GeneralConfigSection");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public DefaultAccountConfigSection DefaultAccountConfiguration
        {
            get
            {
                try
                {
                    return (DefaultAccountConfigSection)ConfigurationManager.GetSection("FinanCity/DefaultAccountConfigSection");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}
