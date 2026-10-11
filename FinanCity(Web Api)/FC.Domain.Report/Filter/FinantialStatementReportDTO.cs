using FC.Core;
using FC.Core.Data;

namespace FC.Domain.Report.Filter
{
    public class FinantialStatementReportDTO : QueryAssert<FinantialStatementReportDTO>
    {
        public FinantialStatementReportDTO() : base(CommonConstant.AccountConnection)
        {

            RuleFor(x => x.AccSK).Custom();
            RuleFor(x => x.AccCode).Custom();
            RuleFor(x => x.CatSK).Custom();
            RuleFor(x => x.CatDescription).Custom();
            RuleFor(x => x.AccDescription).Custom();
            RuleFor(x => x.YC).Custom();
            RuleFor(x => x.PC).Custom();
            RuleFor(x => x.TransactionAmount).Custom();
        }

        public string AccSK { get; set; }

        public string AccCode { get; set; }

        public string CatSK { get; set; }

        public string CatDescription { get; set; }

        public string AccDescription { get; set; }

        public string YC { get; set; }

        public string PC { get; set; }

        public decimal TransactionAmount { get; set; }
    }
}
