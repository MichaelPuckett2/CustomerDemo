namespace CustomerDemoApi.Models;

public record Customer(string FirstName, string LastName, string Email, string Phone);
public record Order(string Description);