namespace FC.Domain.Report
{
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.Business;

    public class JobFor : AbstractFormulaField
    {
        private string _result = $"{SalesViewType.All.ToDescription()}";
        private Job _job;

        public JobFor(Job job)
        {
            _job = job;
        }

        public override string WriteFormat(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    _result = _job.Finds(filter: f => f.SysKey == value.ToString()).Single().Name;
            }
            return _result;
        }
    }
}
