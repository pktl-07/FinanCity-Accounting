namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;

    public class AccountCodeFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private AccountSetup _account;

        public AccountCodeFor(AccountSetup account)
        {
            _account = account;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _account.Finds(filter: f => f.SysKey == value.ToString()).Single().AccCode;
            }
            return _result;
        }
    }
}
