namespace FC.Api.Infrastructure.FluentValidators
{
    using FC.Core;
    using FluentValidation;
    using FC.Api.Models;
    using FC.Domain.Auth.Entity;
   
    public class UserAccountValidator : AbstractValidator<UserAccountEntity>
    {
        #region Ctor

        public UserAccountValidator()
        {
            RuleFor(x => x.Api).NotEmpty();
            RuleFor(x => x.PasswordLoginUserName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.Google).NotEmpty().EmailAddress();
          
            When(x => x.AccountType == (int)UserAccountType.SalespersonAccount, () =>
            {
                RuleFor(x => x.SPSK).NotEmpty();
            });
        }

        #endregion
    }

    public class DeleteUserAccountValidator : AbstractValidator<UserAccountEntity>
    {
        #region Ctor

        public DeleteUserAccountValidator()
        {
            RuleFor(x => x.IsDefault)
                .Equal(false)
                .WithMessage("The default account should not be deleted.");
        }

        #endregion
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        #region Ctor

        public ChangePasswordValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.Password != d.CurrentPassword)
                {
                    context.AddFailure("'Current Password' is incorrect.");
                }
            });

            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.NewPassword != d.ConfirmPassword)
                {
                    context.AddFailure("'New Password' and 'Confirm Password' should be same.");
                }
            });
        }

        #endregion

    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordModel>
    {
        #region Ctor

        public ResetPasswordValidator()
        {
            RuleFor(x => x.SysKey).NotEmpty();
            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.NewPassword != d.ConfirmPassword)
                {
                    context.AddFailure("'New Password' and 'Confirm Password' should be same.");
                }
            });
        }

        #endregion
    }
}