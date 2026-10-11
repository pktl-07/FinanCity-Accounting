namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;

    public class TaxFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private Tax _tax;

        public TaxFor(Tax tax)
        {
            _tax = tax;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _tax.Finds(filter: f => f.SysKey == value.ToString()).Single().TaxCode;
            }
            return _result;
        }
    }
}
