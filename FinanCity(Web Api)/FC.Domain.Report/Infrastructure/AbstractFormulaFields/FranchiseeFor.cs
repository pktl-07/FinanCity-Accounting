namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class FranchiseeFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private Company _company;

        public FranchiseeFor(Company company)
        {
            _company = company;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _company.Finds(filter: f => f.SysKey == value.ToString()).Single().CompanyName;
            }
            return _result;
        }
    }
}
