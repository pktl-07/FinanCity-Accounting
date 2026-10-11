namespace FC.Domain.Accounts.DTO
{
    using System;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;

    public class ChartOfAccountDTO : QueryAssert<ChartOfAccountDTO>
    {
        #region Ctor

        public ChartOfAccountDTO() : base(CommonConstant.AccountConnection)
        {
            RuleFor(x => x.AccountSK).Custom("tbl");
            RuleFor(x => x.AccountCode).Custom("tbl");
            RuleFor(x => x.AccountName).Custom("tbl");
            RuleFor(x => x.CategoryDescription).Custom("tbl");
            RuleFor(x => x.Debit).Custom("tbl");
            RuleFor(x => x.Credit).Custom("tbl");
            RuleFor(x => x.TransactionAmount).Custom("tbl");
            RuleFor(x => x.Index).Custom("tbl");
            RuleFor(x => x.AccountType).Custom("tbl");
            RuleFor(x => x.IsActive).Custom("tbl");
            RuleFor(x => x.GroupHeaderType).Custom("tbl");
            RuleFor(x => x.JobSK).Custom("tbl");
            RuleFor(x => x.JobNo).Custom("tbl");
            RuleFor(x => x.JobName).Custom("tbl");
            RuleFor(x => x.CRDR).Custom("tbl");
        }

        #endregion

        #region Properties

        public string AccountSK { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string CategoryDescription { get; set; }

        public string ControlAccountDescription
        {
            get
            {
                if (IsControlDefault)
                {
                    return DI.Current.Get<IEssAppRepository>()
                        .DefaultAccountingCode.GetAllElements()[AccountSK];
                }
                return string.Empty;
            }
        }

        public bool IsControlDefault
        {
            get
            {
                return DI.Current.Get<IEssAppRepository>()
                        .DefaultAccountingCode.GetAllElements().Keys.Contains(AccountSK);
            }
        }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal TransactionAmount
        {
            get
            {
                if (this.AccountCode.StartsWith("1") || this.AccountCode.StartsWith("5")
                    || this.AccountCode.StartsWith("6") || this.AccountCode.StartsWith("9"))
                {
                    return Debit.ToRoundUp() - Credit.ToRoundUp();
                }
                if (this.AccountCode.StartsWith("2")
                    || this.AccountCode.StartsWith("3")
                    || this.AccountCode.StartsWith("4")
                    || this.AccountCode.StartsWith("8"))
                {
                    return Credit.ToRoundUp() - Debit.ToRoundUp();
                }

                return Math.Abs(Debit.ToRoundUp() - Credit.ToRoundUp());
            }
        }

        public int Index { get; set; }

        public int AccountType { get; set; }

        public bool IsActive { get; set; }

        public int GroupHeaderType { get; set; }

        public string JobSK { get; set; }

        public string JobNo { get; set; }

        public string JobName { get; set; }

        public string CRDR { get; set; }

        #endregion
    }
}
