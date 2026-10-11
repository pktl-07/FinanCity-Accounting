namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FC.Core;
    using FluentValidation;
    using FC.Api.AccountAreas.Models;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using Ninject;
    using System.Linq;

    public class AccountingPeriodValidator : AbstractValidator<AccountingPeriodEntity>
    {
        public AccountingPeriodValidator()
        {
           
        }
    }

    public class AccountingPeriodUpdateValidator : AbstractValidator<AccountingPeriodUpdateModel>
    {
        private BatchModule _batchModule;
        public AccountingPeriodUpdateValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
                FakeModel(d);

                var batchModule = _batchModule.Finds(filter: f => f.YC == d.YC && f.PC == d.PC && f.ModuleCode == d.ModuleCode).Single();
                if (batchModule.EntryStatus == (int)EntryStatus.UnChangeClose)
                {
                    context.AddFailure("This Finantial Year is closed and can not be updated.");
                }
            });
        }

        private void FakeModel(AccountingPeriodUpdateModel model)
        {
            _batchModule = DI.Current.Get<BatchModule>();
        }
    }
}