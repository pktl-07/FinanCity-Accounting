namespace FC.Domain.Report
{
    using FC.Core;
    using System;

    public class ReceivableStatusFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private int _count = 0;

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                var status = (SalesViewType)int.Parse(value.ToString());
                if (status != SalesViewType.All)
                {
                    if (_count == 0)
                        _result = $"{status.ToDescription()}";
                    else
                        _result += $", {status.ToDescription()}";

                    _count++;
                }
            }

            return _result;
        }
    }
}
