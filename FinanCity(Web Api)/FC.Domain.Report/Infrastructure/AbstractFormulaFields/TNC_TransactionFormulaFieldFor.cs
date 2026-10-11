namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class TNC_TransactionFormulaFieldFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private TNC_TransactionType _transactionType;

        public TNC_TransactionFormulaFieldFor(TNC_TransactionType transactionType)
        {
            _transactionType = transactionType;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _transactionType.Finds(filter: f => f.SysKey == value.ToString()).Single().Description;
            }
            return _result;
        }
    }
}
