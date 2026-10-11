namespace FC.Core.Exception
{
    using System;

    public class NotFoundException : Exception
    {
        private string _errorMessage;
        private string _parameterName;

        public NotFoundException(string parameterName, string errorMessage)
        {
            _errorMessage = errorMessage ?? Resources.TextResource.NotFound.FormatWith(parameterName);
            _parameterName = parameterName;
        }

        public override string StackTrace { get { return _parameterName; } }
        
        public override string Message { get { return _errorMessage; } }
    }
}
