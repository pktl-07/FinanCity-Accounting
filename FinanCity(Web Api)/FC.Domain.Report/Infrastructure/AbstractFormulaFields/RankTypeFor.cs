namespace FC.Domain.Report
{
    using FC.Core;
    using System;

    public class RankTypeFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private int _count = 0;

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                var status = int.Parse(value.ToString());
                if (status != 0)
                {
                    if (_count == 0)
                        _result = $"{((RankType)status).ToDescription()}";
                    else
                        _result += $", {((RankType)status).ToDescription()}";

                    _count++;
                }
            }

            return _result;
        }
    }
}
