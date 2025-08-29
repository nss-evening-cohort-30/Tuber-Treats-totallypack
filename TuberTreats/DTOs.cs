namespace TuberTreats.DTOs;

// DTO for creating a new TuberOrder
public class CreateTuberOrderDto
{
    public int CustomerId { get; set; }
    public List<int> ToppingIds { get; set; } = new List<int>();
}

// DTO for assigning a driver to an order
public class AssignDriverDto
{
    public int TuberDriverId { get; set; }
}

// DTO for creating a new Customer
public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

// DTO for adding a topping to an order
public class AddToppingToOrderDto
{
    public int TuberOrderId { get; set; }
    public int ToppingId { get; set; }
}
