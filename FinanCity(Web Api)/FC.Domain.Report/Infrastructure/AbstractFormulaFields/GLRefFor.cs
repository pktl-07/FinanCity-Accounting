namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Sales.Business;

    public class GLRefFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
      
        public GLRefFor()
        {
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = (string)value;
            }
            return _result;
        }
    }
}
