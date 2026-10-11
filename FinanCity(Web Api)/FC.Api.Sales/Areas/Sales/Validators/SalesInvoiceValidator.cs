namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FluentValidation;
  
    public class SalesInvoiceValidator : AbstractValidator<SalesInvoiceModel>
    {
        #region Ctor

        public SalesInvoiceValidator()
        {
            RuleFor(x => x.SalesInvoice.InvoiceDate).NotEmpty();
            RuleFor(x => x.SalesInvoice.PaymentTermSK).NotEmpty();

        }

        #endregion
    }
}