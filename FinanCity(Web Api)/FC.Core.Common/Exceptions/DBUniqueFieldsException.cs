namespace FC.Core.Exceptions
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Infrastructure;

    public class DBUniqueFieldsException : Exception
    {
        public DBUniqueFieldsException()
        {
            // do nothing
        }

        public override string Message
        {
            get
            {
                Guard.IsNotNull(_errorContext, "ErrorContext");
              
                var result = "";
              
                for (int i = 0; i < _errorContext.Count; i++)
                {
                    if (i > 0)
                        result += " or ";

                    result += $"{_errorContext[i].Data.ToUpper()}'s {_errorContext[i].Display}";
                }
                result += " shuould not be duplicated. Please provide other different keys (or) words";
                return result;
            }
        }

        private List<UniqueFieldError> _errorContext;
        public List<UniqueFieldError> ErrorContext
        {
            get
            {
                return _errorContext;
            }
            set
            {
                _errorContext = value;
            }
        }
    }

    public class UniqueFieldError
    {
        public string Data { get; set; }
        public string Display { get; set; }
    }
}
