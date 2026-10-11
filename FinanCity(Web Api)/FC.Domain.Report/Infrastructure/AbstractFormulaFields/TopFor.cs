namespace FC.Domain.Report
{
    using FC.Domain.Sales.Business;

    public class TopFor : AbstractFormulaField
    {
        private int _result = 0;
        private Person _person;

        public TopFor(Person person)
        {
            _person = person;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = int.Parse(value.ToString());
            }

            return _result.ToString();
        }
    }
}
