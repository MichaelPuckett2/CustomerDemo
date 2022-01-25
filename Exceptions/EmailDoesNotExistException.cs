namespace CustomerDemoApi.Exceptions;

public class EmailDoesNotExistException : Exception
{
    public EmailDoesNotExistException(string email) : base($"{email} was not found.") { }
}
