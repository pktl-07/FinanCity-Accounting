namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using FC.Core;
    using Ninject;
    using FluentValidation;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Business;
 
    public class CommissionReceivedAllocationValidator : AbstractValidator<AllocationEntity>
    {
        #region Fields

        private AllocationEntity _model;
        private CommissionReceived _commissionReceived;
        private CommissionAllocation _allocation;
        private PropertyTransactionDetail _propertyDetail;

        #endregion

        #region Ctor

        public CommissionReceivedAllocationValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x.HeaderSK).NotEmpty();
            RuleFor(x => x.SPSK).NotEmpty();
            RuleFor(x => x.DetailSK).NotEmpty();
            RuleFor(x => x.AllocateAmount).GreaterThan(0).WithMessage($"'Total Amount Allocated' must be greater than {0.ToString("c")}");
            RuleFor(x => x.AmountBeforeGST).GreaterThan(0).WithMessage($"'Amount Before GST' must be greater than {0.ToString("c")}");
            RuleFor(x => x.TransType).NotEmpty();
            RuleFor(x => x.TransType).Equal((int)TransType.AllocationCommission);

            When(x => x != null
            && !string.IsNullOrWhiteSpace(x.SPSK)
            && !string.IsNullOrWhiteSpace(x.HeaderSK), () =>
            {
                RuleFor(x => x).Custom((m, context) =>
                {
                    if (m.AllocateAmount > MaxBalance)
                    {
                        context.AddFailure($"'Total Amount Allocated' must be less than or equal {MaxBalance.ToRoundUp().ToString("c")}.");
                    }

                    var balance = 0.0M;
                    if (IsExceedingAllocateAmount(ref balance))
                    {
                        context.AddFailure($"'Total Amount Allocated' must be less than or equal {balance.ToString("c")}");
                    }
                });
            });
        }

        #endregion

        #region Utility

        private void FakeModel(AllocationEntity model)
        {
            _model = model;

            _allocation = DI.Current.Get<CommissionAllocation>();
            _commissionReceived = DI.Current.Get<CommissionReceived>();
            _propertyDetail = DI.Current.Get<PropertyTransactionDetail>();
        }

        private decimal MaxBalance
        {
            get
            {
                var detail = _propertyDetail.Finds(x => x.PTOSK == _model.HeaderSK
                && x.SPSK == _model.SPSK
                && x.RecordStatus == (int)RecordStatus.Post).Single();

                var totalAllocate = _allocation.Finds(x => x.HeaderSK == _model.HeaderSK
                && x.SPSK == _model.SPSK
                && x.RecordStatus == (int)RecordStatus.Post)
                    .Select(x => new { AllocateAmount = x.AllocateAmount.ToRoundUp() })
                    .Sum(x => x.AllocateAmount.ToRoundUp());

                //var credit = detail.Credit.ToRoundUp() + ((detail.Credit.ToRoundUp() / 100) * detail.TaxPercent);
                //var debit = detail.Debit.ToRoundUp() + ((detail.Debit.ToRoundUp() / 100) * detail.TaxPercent);

                return (detail.TotalPayable.ToRoundUp()) - totalAllocate.ToRoundUp();
            }
        }

        private bool IsExceedingAllocateAmount(ref decimal amount)
        {
            var totalReceived = _commissionReceived.Finds(filter: f => f.InvoiceSK == _model.HeaderSK && f.RecordStatus == (int)RecordStatus.Post)
                .Sum(x => x.TotalAmt.ToRoundUp());
            var totalAllocated = _allocation.Finds(filter: f => f.HeaderSK == _model.HeaderSK && f.RecordStatus == (int)RecordStatus.Post)
                .Sum(x => x.AllocateAmount.ToRoundUp());

            var current = _model.AllocateAmount.ToRoundUp();
            var result = totalReceived < totalAllocated + current;

            amount = totalReceived - totalAllocated;

            return result;
        }

        #endregion
    }
}