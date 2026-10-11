namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using FluentValidation;
    using Ninject;
    using FC.Core;

    public class CommissionReceivableValidator : AbstractValidator<CommissionReceivedEntity>
    {
        #region Fields

        private CommissionReceived _commissionReceived;
        private SalesInvoice _invoice;
        private CommissionReceivedEntity _model;
        private decimal _balance;
        private decimal _gst;

        #endregion

        #region Ctor

        public CommissionReceivableValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x.InvoiceSK).NotEmpty();

            RuleFor(x => x.ReceiptDate).NotNull().NotEmpty();

            RuleFor(x => x.CBSK).NotNull().NotEmpty();

            RuleFor(x => x.AmountBeforeGST).NotNull().NotEmpty();

            RuleFor(x => x.TotalAmt).NotNull().NotEmpty();

            When(x => x != null && !string.IsNullOrWhiteSpace(x.InvoiceSK), () =>
            {
                RuleFor(x => x).Custom((m, context) =>
                {
                    if (_balance > 0 && m.TotalAmt > _balance)
                        context.AddFailure($"'Total Amount' must be less than {_balance.ToRoundUp().ToString("c")}.");

                    if (m.GSTPercent / 1 != _gst / 1)
                        context.AddFailure($"'GST' must be {_gst} %.");
                });
            });
        }

        #endregion

        #region Utilities

        private void SetBalance()
        {
            if (_model != null && !string.IsNullOrWhiteSpace(_model.InvoiceSK))
                _balance = _commissionReceived.GetPager(currentPage: 1, pageSize: 10, where: f => f.InvoiceSK == _model.InvoiceSK).Data.ToArray()[0].Balance;
        }

        private void SetInvoiceGST()
        {
            if (_model != null && !string.IsNullOrWhiteSpace(_model.InvoiceSK))
                _gst = _invoice.Finds(filter: f => f.SysKey == _model.InvoiceSK).ToArray()[0].ActualGST;
        }

        private void FakeModel(CommissionReceivedEntity model)
        {
            _model = model;
            _commissionReceived = DI.Current.Get<CommissionReceived>();
            _invoice = DI.Current.Get<SalesInvoice>();
            SetInvoiceGST();
            SetBalance();
        }

        #endregion
    }
}