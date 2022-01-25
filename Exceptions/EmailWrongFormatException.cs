namespace CustomerDemoApi.Exceptions;

public class EmailWrongFormatException : Exception
{
    public EmailWrongFormatException(string email) : base($"{email} is not a valid email address.") { }
}
