namespace FC.Core
{
    public class ReportConfigurator
    {
        #region Fields

        private string _rptFileName;

        #endregion

        #region Members

        public delegate void IntializeRptFileName(string rptFileName);

        public event IntializeRptFileName OnIntializeRptFileName;

        public string RptFileName
        {
            get { return _rptFileName; }
            set
            {
                //Notify new value set
                OnIntializeRptFileName(value);
                _rptFileName = value;
            }
        }

        public Attachment Attachment { get; set; }

        public string Connection { get; set; }

        #endregion
    }
}
