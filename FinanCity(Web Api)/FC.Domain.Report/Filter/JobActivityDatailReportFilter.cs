namespace FC.Domain.Report.Filter
{
    using FC.Core;
    using FC.Core.Data;

    public class JobActivityDatailReportFilter : QueryAssert<JobActivityDatailReportFilter>
        , ISaleDateFor
        , IJobSK
    {
        public JobActivityDatailReportFilter() : base(CommonConstant.SalesConnection)
        {

            RuleFor(x => x.SalesDate).Custom();
            RuleFor(x => x.JobSK).Custom();
        }

        public int? SalesDate
        {
            get; set;
        }
       
        public string JobSK
        {
            get;
            set;
        }
    }
}
