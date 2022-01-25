using System.Reflection;

namespace CustomerDemoApi.Mocks;

public class MockCustomerContext : ICustomerDB, IDisposable
{
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly IValidateEmail validateEmail;
    private readonly IValidatePhoneNumber validatePhoneNumber;
    private static readonly ICollection<Models.Entities.Customer> customers = new HashSet<Models.Entities.Customer>
    {
        new() { Id = 1, FirstName = "Michael", LastName = "Puckett", Email = "michael.puckett@tripleg3.com", Phone = "707.592.2149" },
        new() { Id = 2, FirstName = "Erin", LastName = "Puckett", Email = "erin.puckett@tripleg3.com", Phone = "209-482-2195" },
        new() { Id = 3, FirstName = "Cory", LastName = "Powell", Email = "cory.powell@tripleg3.com", Phone = "707.482.2121" }
    };

    private static readonly ICollection<Models.Entities.Order> orders = new HashSet<Models.Entities.Order>
    {
        new() { Id = 1, Description = "Chips" },
        new() { Id = 2, Description = "Cookies" },
        new() { Id = 3, Description = "Monster" }
    };

    private static readonly ICollection<Models.Entities.CustomerOrder> customerOrders = new HashSet<Models.Entities.CustomerOrder>();

    public MockCustomerContext(IValidateEmail validateEmail,
                               IValidatePhoneNumber validatePhoneNumber)
    {
        this.validateEmail = validateEmail;
        this.validatePhoneNumber = validatePhoneNumber;
    }

    public async Task<IList<Customer>> GetCustomersAsync(int skip = int.MinValue, int take = int.MaxValue)
    {
        await semaphoreSlim.WaitAsync();
        IList<Customer> customers = MockCustomerContext
            .customers
            .Skip(skip)
            .Take(take)
            .Select(x => x.As<Customer>())
            .ToList();

        semaphoreSlim.Release();
        return customers;
    }


    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        var email = customer.Email.ToLowerInvariant();

        if (!validateEmail.Validate(email))
        {
            throw new EmailWrongFormatException(customer.Email);
        }

        if (!validatePhoneNumber.Validate(customer.Phone))
        {
            throw new PhoneNumberWrongFormatException(customer.Phone);
        }

        await semaphoreSlim.WaitAsync();

        if (MockCustomerContext.customers.Any(x => x.Email?.ToLowerInvariant() == email))
        {
            semaphoreSlim.Release();
            throw new EmailAlreadyExistsException(customer.Email);
        }

        MockCustomerContext.customers.Add(customer
            .Map<Customer, Models.Entities.Customer>()
            .MapCustom(new IdMap())
            .Build());

        semaphoreSlim.Release();
        return customer;
    }

    public async Task<Customer> DeleteCustomerWithEmailAsync(string email)
    {
        email = email.ToLowerInvariant();

        if (!validateEmail.Validate(email))
        {
            throw new EmailWrongFormatException(email);
        }

        await semaphoreSlim.WaitAsync();

        Models.Entities.Customer? customer = MockCustomerContext
            .customers
            .FirstOrDefault(x => x.Email?.ToLowerInvariant() == email.ToLowerInvariant());

        if (customer is null)
        {
            semaphoreSlim.Release();
            throw new EmailDoesNotExistException(email);
        }

        if (customerOrders.Any(x => x.CustomerId == customer.Id))
        {
            throw new CannotDeleteCustomerException($"{customer.Email} cannot be removed because there are associated ordered.");
        }

        MockCustomerContext.customers.Remove(customer);
        semaphoreSlim.Release();
        return customer.As<Customer>();
    }

    class IdMap : ImmutaMap.Interfaces.IMapping
    {
        public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object? result)
        {
            if (sourcePropertyInfo.Name == nameof(Models.Entities.Customer.Id))
            {
                result = MockCustomerContext.customers.Count;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }

    public void Dispose()
    {
        semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }

    ~MockCustomerContext() => Dispose();
}