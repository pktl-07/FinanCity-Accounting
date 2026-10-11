namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using System.Linq;
    using FC.Domain.Accounts.Entity;
    using FluentValidation;
    using Ninject;
    using FC.Core;
    using FC.Domain.Sales.Business;
  
    public class DepartmentSetupValidator : AbstractValidator<DepartmentEntity>
    {
        #region Ctor

        public DepartmentSetupValidator()
        {
            RuleFor(x => x.R1).NotEmpty().WithMessage("'Department Code' is required");
            RuleFor(x => x.Description).NotEmpty();
        }

        #endregion
    }

    public class DepartmentSetupDeleteValidator : AbstractValidator<string>
    {
        #region Ctor

        public DepartmentSetupDeleteValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
                var p = DI.Current.Get<Person>();

                if (p.Finds(filter: f => f.DepartmentSK == d).Count() > 0)
                    context.AddFailure("'Department' should not be deleted.");
            });
        }

        #endregion
    }
}