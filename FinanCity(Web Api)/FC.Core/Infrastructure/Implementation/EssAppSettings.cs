namespace FC.Core.Infrastructure.Implementation
{
    public class EssAppServices : IEssAppRepository
    {
        #region Fields

        private int _currentUtc;
        private string _dateFormat;
        private LetterCasing _letterFormat;

        #endregion

        #region Members

        public int CurrentUtc
        {
            get { return _currentUtc; }
            set
            {
                _currentUtc = value;
            }
        }

        public string DateFormat
        {
            get { return _dateFormat; }
            set
            {
                Guard.IsNotNullOrEmpty(value, "DateFormat");
                _dateFormat = value;
            }
        }

        public LetterCasing LetterFormat
        {
            get { return _letterFormat; }
            set { _letterFormat = value; }
        }

        public int RemoveLeadWhoNeverClientForDays { get; set; }

        public string CURRSK { get; set; }

        public decimal CURRATE { get; set; }

        public string CURROP { get; set; }

        public string SPSK { get; set; }

        public string ResourceUrl { get; set; }

        public string DefaultCountry { get; set; }

        public string DomainUrl { get; set; }

        public DefaultCompanyTax DefaultSalesTax { get; set; }

        public DefaultCompanyTax DefaultPurchaseTax { get; set; }

        public DefaultCompanyTax DefaultPurchaseZeroTax { get; set; }

        public string FrontEndUrl
        {
            get;
            set;
        }

        public DefaultAccountingCode DefaultAccountingCode
        {
            get;
            set;
        }

        public string CURRCODE
        {
            get;
            set;
        }

        public string CompanyName
        {
            get;
            set;
        }

        public string ReportTemplateDirectory
        {
            get;
            set;
        }

        public string SharedReportTemplateDirectory
        {
            get;
            set;
        }

        public bool EnableSalespersonTierBonus
        {
            get;
            set;
        }

        public string AppVersionInfo
        {
            get;
            set;
        }

        #endregion
    }
}
