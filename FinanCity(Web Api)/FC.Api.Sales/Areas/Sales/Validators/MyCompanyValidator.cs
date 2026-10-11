namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;
  
    public class MyCompanyValidator : AbstractValidator<CompanyAndContactModel>
    {
        #region Ctor

        public MyCompanyValidator()
        {
            RuleFor(x => x.Vendor.CompanyName).NotEmpty();
            RuleFor(x => x.Vendor.CompanyReg).NotEmpty();
            RuleFor(x => x.Vendor.AgentLicense).NotEmpty();
            //RuleFor(x => x.Vendor.TaxSK).NotEmpty();
        }

        #endregion
    }
}