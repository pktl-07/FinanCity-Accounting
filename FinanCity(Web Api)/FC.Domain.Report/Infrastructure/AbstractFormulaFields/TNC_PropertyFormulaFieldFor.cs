namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class TNC_PropertyFormulaFieldFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private TNC_PropertyType _propertyType;

        public TNC_PropertyFormulaFieldFor(TNC_PropertyType propertyType)
        {
            _propertyType = propertyType;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _propertyType.Finds(filter: f => f.SysKey == value.ToString()).Single().Description;
            }
            return _result;
        }
    }
}
