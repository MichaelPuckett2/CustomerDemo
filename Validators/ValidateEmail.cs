using System.Text.RegularExpressions;

namespace CustomerDemoApi.Validators
{
    public class ValidateEmail : IValidateEmail
    {
        public bool Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            return Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        }
    }
}
