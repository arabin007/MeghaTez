using FluentValidation;

namespace Application.CustomValidator
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var opt = ruleBuilder.NotEmpty()
                                 .MinimumLength(6).WithMessage("Password must be atleat 6 characters.")
                                 .Matches("[A-Z]").WithMessage("Password must contain Uppercase character.")
                                 .Matches("[a-z]").WithMessage("Password must contain Lowercase character.")
                                 .Matches("[0-9]").WithMessage("Password must contain number.")
                                 .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain alphanumeric character.");
            return opt;
        }
    }
}
