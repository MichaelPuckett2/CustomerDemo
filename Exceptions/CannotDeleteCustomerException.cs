namespace CustomerDemoApi.Exceptions;

public class CannotDeleteCustomerException : Exception
{
    public CannotDeleteCustomerException(string message) : base(message) { }
}
