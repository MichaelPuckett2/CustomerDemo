namespace CustomerDemoApi.Exceptions;

public class PhoneNumberWrongFormatException : Exception
{
    public PhoneNumberWrongFormatException(string phoneNumber) : base($"{phoneNumber} is not a valid phone number.") { }
}
