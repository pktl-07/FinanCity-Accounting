namespace FC.Domain.Report
{
    using FC.Core;

    public class SecondRefFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";

        public SecondRefFor()
        {
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = (string)value;
            }
            return _result;
        }
    }
}
