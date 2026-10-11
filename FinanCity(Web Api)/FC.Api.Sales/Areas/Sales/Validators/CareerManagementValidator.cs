namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Core;
    using FluentValidation;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using Ninject;
    using System.Linq;

    public class CareerManagementValidator : AbstractValidator<CareerManagementEntity>
    {
        public CareerManagementValidator()
        {
            RuleFor(x => x.EffectiveDate).NotEmpty();
            RuleFor(x => x.CreateDate).NotEmpty();
            RuleFor(x => x.SPSK).NotEmpty();
            RuleFor(x => x.ApprovedBy).NotEmpty();

            When(x => x.TransType == (int)TransType.BranchOut, () =>
            {
                RuleFor(x => x).Custom((m, context) =>
                {
                    if (m.SPSK == m.T1)
                        context.AddFailure("'Salesperson' is invalid.");
        
                    var person = DI.Current.Get<Person>();

                    if (!string.IsNullOrWhiteSpace(m.T1))
                    {
                        var a = person.GetSalespersonRecursiveTree(key: m.T1,
                            source: person.GetSalepersonTire(),
                            downTree: false,
                            reverse: true);

                        if (a.Traverse(x => x.Items).Where(x => x.SPSK == m.SPSK).Count() > 0)
                            context.AddFailure("'Salesperson' circular dependency occours. Please choose the valid salesperson.");
                    }
                });
            });
        }
    }
}