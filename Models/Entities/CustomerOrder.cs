namespace CustomerDemoApi.Models.Entities;

public class CustomerOrder
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
}