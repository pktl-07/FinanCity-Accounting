namespace FC.Domain.Report
{
    using FC.Core;
   
    public class SalespersonTypeFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";

        public SalespersonTypeFor()
        {
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (int.Parse(value.ToString()) == (int)TransType.InternalSalesperson)
                    _result = TransType.InternalSalesperson.ToDescription();
                else if (int.Parse(value.ToString()) == (int)TransType.ExternalSalesPerson)
                    _result = TransType.ExternalSalesPerson.ToDescription();
            }

            return _result;
        }
    }
}
