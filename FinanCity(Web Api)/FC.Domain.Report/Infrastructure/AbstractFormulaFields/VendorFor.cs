namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class VendorFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private Company _company;

        public VendorFor(Company person)
        {
            _company = person;
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
