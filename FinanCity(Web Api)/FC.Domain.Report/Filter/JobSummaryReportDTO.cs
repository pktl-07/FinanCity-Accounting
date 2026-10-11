namespace FC.Domain.Report.Filter
{
    using FC.Core.Data;

    public class JobSummaryReportDTO : QueryAssert<JobSummaryReportDTO>
    {
        public JobSummaryReportDTO() : base("FC.Acc")
        {
            RuleFor(x => x.AccSK);
            RuleFor(x => x.TransactionAmount);
            RuleFor(x => x.AccCode);
            RuleFor(x => x.AccDescription);
            RuleFor(x => x.JobSK);
            RuleFor(x => x.JobNo);
            RuleFor(x => x.JobName);
        }

        public string AccSK { get; set; }

        public decimal TransactionAmount { get; set; }

        public string AccCode { get; set; }

        public string AccDescription { get; set; }

        public string JobSK { get; set; }

        public string JobNo { get; set; }

        public string JobName { get; set; }
    }
}
