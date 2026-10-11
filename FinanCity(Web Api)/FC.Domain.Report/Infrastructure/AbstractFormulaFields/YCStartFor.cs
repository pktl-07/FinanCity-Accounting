namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;
  
    public class YCStartFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private AccountingPeriod _accountingPeriod;

        public YCStartFor(AccountingPeriod accountingPeriod)
        {
            _accountingPeriod = accountingPeriod;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var period = _accountingPeriod.Finds(filter: f => f.YC == value.ToString()).First();

                    _result = $"{period.YC} [{period.Date1.ToString().Substring(0, 4)}]";
                }
            }
            return _result;
        }
    }
}
