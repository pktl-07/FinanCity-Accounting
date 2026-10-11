namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class T1For : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private Person _person;

        public T1For(Person person)
        {
            _person = person;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _person.Finds(filter: f => f.SysKey == value.ToString()).Single().Name;
            }
            return _result;
        }
    }
}
