namespace FC.Domain.Report
{
    using FC.Core;
 
    public class RecordStatusFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private int _count = 0;

        public RecordStatusFor()
        {
        }

        public override string WriteFormat(object value)
        {
            //if (value != null)
            //    _result = ((RecordStatus)int.Parse(value.ToString())).ToDescription();
            if (value != null)
            {
                var status = (RecordStatus)int.Parse(value.ToString());
                if (status != (int)SalesViewType.All)
                {
                    if (_count == 0)
                        _result = $"{status.ToDescription()}";
                    else
                        _result += $", {status.ToDescription()}";

                    _count++;
                }
            }

            return _result.ToString();
        }
    }
}
