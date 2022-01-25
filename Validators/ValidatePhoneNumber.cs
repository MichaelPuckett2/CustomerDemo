using System.Text.RegularExpressions;

namespace CustomerDemoApi.Validators
{
    public class ValidatePhoneNumber : IValidatePhoneNumber
    {
        public bool Validate(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }
            var pattern = @"^[\+]?[{1}]?[(]?[2-9]\d{2}[)]?[-\s\.]?[2-9]\d{2}[-\s\.]?[0-9]{4}$";
            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            return Regex.IsMatch(phoneNumber, pattern, options);
        }
    }
}
