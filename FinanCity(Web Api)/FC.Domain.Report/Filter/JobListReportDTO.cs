using FC.Core;
using FC.Core.Data;
using System;

namespace FC.Domain.Report.Filter
{
    public class JobListReportDTO : QueryAssert<JobListReportDTO>
    {
        public JobListReportDTO() : base(CommonConstant.AccountConnection)
        {

            RuleFor(x => x.JobName).Custom();
            RuleFor(x => x.JobSK).Custom();
            RuleFor(x => x.JobNo).Custom();
            RuleFor(x => x.Income).Custom();
            RuleFor(x => x.COS).Custom();
            RuleFor(x => x.Expense).Custom();
            RuleFor(x => x.OtherIncome).Custom();
            RuleFor(x => x.OtherExpense).Custom();
            RuleFor(x => x.NetProfit).Custom();
        }
        public string JobName { get; set; }

        public string JobSK { get; set; }

        public string JobNo { get; set; }

        public decimal Income { get; set; }

        public decimal COS { get; set; }

        public decimal Expense { get; set; }

        public decimal OtherIncome { get; set; }

        public decimal OtherExpense { get; set; }

        public decimal NetProfit { get; set; }
    }
}
