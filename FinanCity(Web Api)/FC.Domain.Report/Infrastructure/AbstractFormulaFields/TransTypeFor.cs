namespace FC.Domain.Report
{
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class TransTypeFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
      
        public TransTypeFor()
        {
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                _result = ((TransType)(int)value).ToDescription();
            }

            return _result.ToString();
        }
    }
}
