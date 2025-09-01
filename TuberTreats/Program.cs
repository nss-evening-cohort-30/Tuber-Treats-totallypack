using System.Text.Json.Serialization;
using TuberTreats.DTO;
using TuberTreats.Models;

var builder = WebApplication.CreateBuilder(args);

List<Customer> customers =
[
    new Customer { Id = 1, Name = "John Doe", Address = "123 Main St" },
    new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak Ave" },
    new Customer { Id = 3, Name = "Bob Johnson", Address = "789 Pine Rd" },
    new Customer { Id = 4, Name = "Alice Brown", Address = "321 Elm St" },
    new Customer { Id = 5, Name = "Charlie Davis", Address = "654 Maple Dr" }
];

List<TuberDriver> drivers =
[
    new TuberDriver { Id = 1, Name = "Frank Miller" },
    new TuberDriver { Id = 2, Name = "Grace Lee" },
    new TuberDriver { Id = 3, Name = "Henry Adams" }
];

List<TuberOrder> orders =
[
    new TuberOrder { Id = 1, OrderPlacedOnDate = DateTime.Now.AddDays(-2), CustomerId = 1, TuberDriverId = 1 },
    new TuberOrder { Id = 2, OrderPlacedOnDate = DateTime.Now.AddDays(-1), CustomerId = 2, TuberDriverId = 2, DeliveredOnDate = DateTime.Now.AddHours(-2) },
    new TuberOrder { Id = 3, OrderPlacedOnDate = DateTime.Now.AddHours(-3), CustomerId = 3 }
];

List<Topping> toppings =
[
    new Topping { Id = 1, Name = "Butter" },
    new Topping { Id = 2, Name = "Sour Cream" },
    new Topping { Id = 3, Name = "Chives" },
    new Topping { Id = 4, Name = "Bacon Bits" },
    new Topping { Id = 5, Name = "Cheese" }
];

List<TuberTopping> tuberToppings =
[
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 2 },
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 3 },
    new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 },
    new TuberTopping { Id = 5, TuberOrderId = 3, ToppingId = 5 }
];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

// Configure JSON options for .NET 8
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(options =>
    {
        options.AllowAnyOrigin();
        options.AllowAnyMethod();
        options.AllowAnyHeader();
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Helper method to populate order with related data
TuberOrder PopulateOrderData(TuberOrder order)
{
    var populatedOrder = new TuberOrder
    {
        Id = order.Id,
        OrderPlacedOnDate = order.OrderPlacedOnDate,
        CustomerId = order.CustomerId,
        TuberDriverId = order.TuberDriverId,
        DeliveredOnDate = order.DeliveredOnDate
    };

    // Add customer without circular reference
    var customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);
    if (customer != null)
    {
        populatedOrder.Customer = new Customer
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            TuberOrders = [] // Empty to avoid circular reference
        };
    }

    // Add driver without circular reference
    if (order.TuberDriverId.HasValue)
    {
        var driver = drivers.FirstOrDefault(d => d.Id == order.TuberDriverId.Value);
        if (driver != null)
        {
            populatedOrder.TuberDriver = new TuberDriver
            {
                Id = driver.Id,
                Name = driver.Name,
                TuberDeliveries = [] // Empty to avoid circular reference
            };
        }
    }

    // Add tuber toppings with topping data
    populatedOrder.TuberToppings = [.. tuberToppings
        .Where(tt => tt.TuberOrderId == order.Id)
        .Select(tt => new TuberTopping
        {
            Id = tt.Id,
            TuberOrderId = tt.TuberOrderId,
            ToppingId = tt.ToppingId,
            Topping = toppings.FirstOrDefault(t => t.Id == tt.ToppingId)
        })];

    return populatedOrder;
}

// TUBER ORDERS ENDPOINTS
// Get all orders
app.MapGet("/tuberorders", () =>
{
    return orders.Select(PopulateOrderData).ToList();
});

// Get order by id
app.MapGet("/tuberorders/{id}", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();
    return Results.Ok(PopulateOrderData(order));
});

// Create new order
app.MapPost("/tuberorders", (TuberOrder newOrder) =>
{
    int newId = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
    
    newOrder.Id = newId;
    newOrder.OrderPlacedOnDate = DateTime.Now;
    
    orders.Add(newOrder);
    
    return PopulateOrderData(newOrder);
});

// Update order (assign driver)
app.MapPut("/tuberorders/{id}", (int id, TuberOrder updatedOrder) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();
    
    order.TuberDriverId = updatedOrder.TuberDriverId;
    order.CustomerId = updatedOrder.CustomerId;
    
    return Results.Ok(PopulateOrderData(order));
});

// Complete order
app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();
    
    order.DeliveredOnDate = DateTime.Now;
    
    return Results.Ok(PopulateOrderData(order));
});

// TOPPINGS ENDPOINTS
// Get all toppings
app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new Topping { Id = t.Id, Name = t.Name }).ToList();
});

// Get topping by id
app.MapGet("/toppings/{id}", (int id) =>
{
    var topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null) return Results.NotFound();
    return Results.Ok(new Topping { Id = topping.Id, Name = topping.Name });
});

// TUBER TOPPINGS ENDPOINTS
// Get all tuber toppings
app.MapGet("/tubertoppings", () =>
{
    return tuberToppings.Select(tt => new TuberTopping 
    { 
        Id = tt.Id, 
        TuberOrderId = tt.TuberOrderId, 
        ToppingId = tt.ToppingId 
    }).ToList();
});

// Add topping to order
app.MapPost("/tubertoppings", (TuberTopping newTuberTopping) =>
{
    int newId = tuberToppings.Count > 0 ? tuberToppings.Max(tt => tt.Id) + 1 : 1;
    
    newTuberTopping.Id = newId;
    tuberToppings.Add(newTuberTopping);
    
    return new TuberTopping 
    { 
        Id = newTuberTopping.Id, 
        TuberOrderId = newTuberTopping.TuberOrderId, 
        ToppingId = newTuberTopping.ToppingId 
    };
});

// Remove topping from order
app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    var tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (tuberTopping == null) return Results.NotFound();
    
    tuberToppings.Remove(tuberTopping);
    return Results.Ok();
});

// CUSTOMERS ENDPOINTS
// Get all customers
app.MapGet("/customers", () =>
{
    return customers.Select(c => new Customer
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address,
        TuberOrders = [.. orders
            .Where(o => o.CustomerId == c.Id)
            .Select(PopulateOrderData)]
    }).ToList();
});

// Get customer by id with orders
app.MapGet("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();
    
    var customerResult = new Customer
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        TuberOrders = [.. orders
            .Where(o => o.CustomerId == customer.Id)
            .Select(PopulateOrderData)]
    };
    
    return Results.Ok(customerResult);
});

// Add customer
app.MapPost("/customers", (Customer newCustomer) =>
{
    int newId = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
    
    newCustomer.Id = newId;
    customers.Add(newCustomer);
    
    return new Customer { Id = newCustomer.Id, Name = newCustomer.Name, Address = newCustomer.Address };
});

// Delete customer
app.MapDelete("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();
    
    customers.Remove(customer);
    return Results.Ok();
});

// TUBER DRIVERS ENDPOINTS
// Get all drivers
app.MapGet("/tuberdrivers", () =>
{
    return drivers.Select(d => new TuberDriver
    {
        Id = d.Id,
        Name = d.Name,
        TuberDeliveries = [.. orders
            .Where(o => o.TuberDriverId == d.Id)
            .Select(PopulateOrderData)]
    }).ToList();
});

// Get driver by id with deliveries
app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    var driver = drivers.FirstOrDefault(d => d.Id == id);
    if (driver == null) return Results.NotFound();
    
    var driverResult = new TuberDriver
    {
        Id = driver.Id,
        Name = driver.Name,
        TuberDeliveries = [.. orders
            .Where(o => o.TuberDriverId == driver.Id)
            .Select(PopulateOrderData)]
    };
    
    return Results.Ok(driverResult);
});

app.Run();

//don't touch or move this!
public partial class Program { }
