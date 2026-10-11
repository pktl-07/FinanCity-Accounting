namespace FC.Core.Exception
{
    public class NameConventionException : System.Exception
    {
        private string _errorMessage;
        private string _parameterName;

        public NameConventionException(string parameterName, string errorMessage)
        {
            _errorMessage = errorMessage ?? "Name convention expression.";
            _parameterName = parameterName;
        }

        public override string StackTrace { get { return _parameterName; } }

        public override string Message { get { return _errorMessage; } }
    }
}
