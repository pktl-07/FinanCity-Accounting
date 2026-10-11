namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Sales.Business;

    public class CashbookFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private CashbookSetup _cashbook;

        public CashbookFor(CashbookSetup cahsbook)
        {
            _cashbook = cahsbook;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _cashbook.Finds(filter: f => f.SysKey == value.ToString()).Single().CashbookName;
            }
            return _result;
        }
    }
}
