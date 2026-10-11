namespace FC.Core.Exception
{
    public class DupicateKeyException : System.Exception
    {
        private string _errorMessage;
        private string _parameterName;

        public DupicateKeyException(string parameterName, string errorMessage)
        {
            _errorMessage = errorMessage ??  Resources.TextResource.DictionaryKeyShouldNotBeDuplicate.FormatWith(parameterName);
            _parameterName = parameterName;
        }

        public override string StackTrace { get { return _parameterName; } }
        
        public override string Message {  get { return _errorMessage; } }
    }
}
