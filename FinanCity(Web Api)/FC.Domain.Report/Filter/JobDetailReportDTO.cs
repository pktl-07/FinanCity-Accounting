using FC.Core;
using FC.Core.Data;
using System;

namespace FC.Domain.Report.Filter
{
    public class JobDetailReportDTO : QueryAssert<JobDetailReportDTO>
    {
        public JobDetailReportDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.AccSK).Custom();
            RuleFor(x => x.Debit).Custom();
            RuleFor(x => x.Credit).Custom();
            RuleFor(x => x.AccCode).Custom();
            RuleFor(x => x.AccDescription).Custom();
            RuleFor(x => x.JobSK).Custom();
            RuleFor(x => x.JobNo).Custom();
            RuleFor(x => x.JobName).Custom();
            RuleFor(x => x.NetActivity).Custom();
            RuleFor(x => x.SecondRef).Custom();
            RuleFor(x => x.ModuleCode).Custom();
            RuleFor(x => x.PostDate).Custom();
            RuleFor(x => x.Memo).Custom();
        }

        public string AccSK
        {
            get;
            set;
        }

        public decimal Debit
        {
            get;
            set;
        }

        private decimal _credit;
        public decimal Credit
        {
            get { return Math.Abs(_credit); }
            set { _credit = value; }
        }

        public string AccCode { get; set; }

        public string AccDescription { get; set; }

        public string JobSK { get; set; }

        public string JobNo { get; set; }

        public string JobName { get; set; }

        public decimal NetActivity
        {
            get { return Debit - Credit; }
        }

        public string SecondRef { get; set; }

        public string ModuleCode { get; set; }

        private int? _postDate;
        public string PostDate
        {
            get { return GeneralUtility.ConvertDisplayDateFormat(_postDate); }
            set { _postDate = int.Parse(value); }
        }

        public string Memo { get; set; }
    }
}
