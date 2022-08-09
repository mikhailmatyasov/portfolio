using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace WeSafe.Web.Common.Validators
{
    /// <summary>
    /// Custom validators for FluentValidation.
    /// </summary>
    public static class CustomValidators
    {
        /// <summary>
        /// Defines a validator for MAC address string values. Validation will fail if the value does not match the valid MAC address.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> ValidMacAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Matches(Constants.Constants.MacAddressPattern).WithMessage("{PropertyName} is not a valid MAC address");
        }

        /// <summary>
        /// Defines a validator for phone number string values. Validation will fail if the value does not match the valid phone number.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> ValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Matches(Constants.Constants.PhoneNumberPattern).WithMessage("{PropertyName} is not a valid phone number");
        }

        /// <summary>
        /// Defines a validator for password string values.
        /// Validation will fail if the password length less than 6 chars or greater than 50 chars.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.MaximumLength(50)
                              .WithMessage("{PropertyName} is too long")
                              .MinimumLength(6)
                              .WithMessage("{PropertyName} cannot be less than 6 chars");
        }

        /// <summary>
        /// Defines a validator for device token string values.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> ValidDeviceToken<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.NotEmpty()
                              .WithMessage("{PropertyName} is required.")
                              .Must(value => value != null && value.Any(char.IsDigit) && value.Any(char.IsLetter))
                              .WithMessage("{PropertyName} should contains letters and digits.");
        }

        /// <summary>
        /// Defines a validator for client identifier.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidClientId<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0)
                              .WithMessage("Client identifier is required and should be greater than 0.");
        }

        /// <summary>
        /// Defines a validator for device identifier.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidDeviceId<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0)
                              .WithMessage("Device identifier is required and should be greater than 0.");
        }

        /// <summary>
        /// Defines a validator for ip address.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> ValidIpAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Matches(Constants.Constants.IpAddressPattern).WithMessage("{PropertyName} is not a valid ip address"); ;
        }
    }
}