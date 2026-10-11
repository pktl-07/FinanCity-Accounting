namespace FC.Domain.Report
{
    using FC.Core;

    public class CobrokerAccuralChangeDateFor : AbstractFormulaField
    {
        private string _result;

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(_result))
                    _result = $"{GeneralUtility.ConvertDisplayDateFormat(value.ToString())}";
            }

            return _result;
        }
    }
}
