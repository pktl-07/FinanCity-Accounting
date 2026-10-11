namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using FC.Core;
    using FluentValidation;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using Ninject;
 
    public class CommissionPayableValidator : AbstractValidator<CommissionPaymentEntity>
    {
        #region Fields

        private CommissionPaymentEntity _model;
        private PayableScheme _payableScheme;
        private CommissionPayment _commissionPayment;
        private decimal _balance;

        #endregion

        #region Methods

        public CommissionPayableValidator()
        {

            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x.ALOSK).NotEmpty();
            RuleFor(x => x.DatePaid).NotEmpty();
            RuleFor(x => x.CBSK).NotEmpty();

            When(x => x != null && !string.IsNullOrWhiteSpace(x.ALOSK), () =>
            {
                RuleFor(x => x).Custom((m, context) =>
                {
                    if (_balance > 0 && _balance < m.NetPaid)
                        context.AddFailure($"'Payment' must be less than {_balance.ToRoundUp().ToString("c")}.");
                });
            });
        }

        #endregion

        #region Utility

        private void FakeModel(CommissionPaymentEntity model)
        {
            _model = model;
            _payableScheme = DI.Current.Get<PayableScheme>();
            _commissionPayment = DI.Current.Get<CommissionPayment>();

            _balance = _payableScheme.Finds(filter: (f) => f.ALOSK == model.ALOSK && f.RecordStatus == (int)RecordStatus.Post)
                .Sum(x => x.AssociateComm.ToRoundUp()) - _commissionPayment.Finds(filter: (f) => f.ALOSK == model.ALOSK && f.RecordStatus == (int)RecordStatus.Post).Sum(x => x.NetPaid.ToRoundUp());
        }

        #endregion
    }
}