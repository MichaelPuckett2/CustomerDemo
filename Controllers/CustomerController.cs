namespace CustomerDemoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> logger;
    private readonly ICustomerDB customerContext;

    public CustomerController(ILogger<CustomerController> logger,
                              ICustomerDB customerContext)
    {
        this.logger = logger;
        this.customerContext = customerContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        IEnumerable<Customer> customers;
        try
        {
            customers = await customerContext.GetCustomersAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Problem();
        }
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Customer customer)
    {
        Customer newCustomer;
        try
        {
            newCustomer = await customerContext.AddCustomerAsync(customer);
        }
        catch (PhoneNumberWrongFormatException phoneNumberWrongFormatException)
        {
            logger.LogInformation(phoneNumberWrongFormatException.Message);
            return Problem(phoneNumberWrongFormatException.Message);
        }
        catch (EmailWrongFormatException emailWrongFormatException)
        {
            logger.LogInformation(emailWrongFormatException.Message);
            return Problem(emailWrongFormatException.Message);
        }
        catch (EmailAlreadyExistsException emailAlreadyExistException)
        {
            logger.LogInformation(emailAlreadyExistException.Message);
            return UnprocessableEntity(emailAlreadyExistException.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Problem();
        }
        return Ok(newCustomer);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCustomerWithEmailAsync(string email)
    {
        Customer removedCustomer;
        try
        {
            removedCustomer = await customerContext.DeleteCustomerWithEmailAsync(email);
        }
        catch (EmailDoesNotExistException emailDoesNotExistException)
        {
            logger.LogInformation(emailDoesNotExistException.Message);
            return NotFound(emailDoesNotExistException.Message);
        }
        catch (CannotDeleteCustomerException cannotDeleteCustomerException)
        {
            logger.LogInformation(cannotDeleteCustomerException.Message);
            return Problem(cannotDeleteCustomerException.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Problem();
        }
        return Ok(removedCustomer);
    }
}