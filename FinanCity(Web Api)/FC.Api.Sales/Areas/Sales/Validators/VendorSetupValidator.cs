namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FluentValidation;

    public class VendorSetupValidator : AbstractValidator<CompanyAndContactModel>
    {
        #region Ctor

        public VendorSetupValidator()
        {
            RuleFor(x => x.Vendor.CompanyName).NotEmpty();
            RuleFor(x => x.Vendor.CompanyReg).NotEmpty();
        }

        #endregion
    }
}