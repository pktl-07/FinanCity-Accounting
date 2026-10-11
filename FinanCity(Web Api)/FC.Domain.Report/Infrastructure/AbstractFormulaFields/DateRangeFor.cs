namespace FC.Domain.Report
{
    using FC.Core;

    public class DateRangeFor : AbstractFormulaField
    {
        private string _result;

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(_result))
                    _result = $"From {GeneralUtility.ConvertDisplayDateFormat(value.ToString())}";
                else
                    _result += $" To {GeneralUtility.ConvertDisplayDateFormat(value.ToString())}";
            }
            return _result;
        }
    }
}
