namespace CustomerDemoApi.Interfaces;

public interface ICustomerDB
{
    Task<IList<Customer>> GetCustomersAsync(int skip = int.MinValue, int take = int.MaxValue);
    Task<Customer> AddCustomerAsync(Customer customer);
    Task<Customer> DeleteCustomerWithEmailAsync(string email);
}