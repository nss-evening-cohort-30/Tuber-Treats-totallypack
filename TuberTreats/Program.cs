using TuberTreats.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

List<Customer> customers =
[
    new Customer { Id = 1, Name = "John Doe", Address = "123 Main St" },
    new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak Ave" },
    new Customer { Id = 3, Name = "Bob Johnson", Address = "789 Pine Rd" }
];

List<TuberDriver> drivers =
[
    new TuberDriver { Name = "Frank Miller" },
    new TuberDriver { Name = "Grace Lee" },
    new TuberDriver { Name = "Henry Adams" }
];

List<TuberOrder> orders =
[
    new TuberOrder { OrderPlacedOnDate = DateTime.Now.AddDays(-2), CustomerId = 1, TuberDriverId = 1 },
    new TuberOrder { OrderPlacedOnDate = DateTime.Now.AddDays(-1), CustomerId = 2, TuberDriverId = 2, DeliveredOnDate = DateTime.Now.AddHours(-2) },
    new TuberOrder { OrderPlacedOnDate = DateTime.Now.AddHours(-3), CustomerId = 3 }
];

List<Topping> toppings =
[
    new Topping { Name = "Butter" },
    new Topping { Name = "Sour Cream" },
    new Topping { Name = "Chives" },
    new Topping { Name = "Bacon Bits" },
    new Topping { Name = "Cheese" }
];

List<TuberTopping> tuberToppings =
[
    new TuberTopping { TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { TuberOrderId = 1, ToppingId = 2 },
    new TuberTopping { TuberOrderId = 2, ToppingId = 3 },
    new TuberTopping { TuberOrderId = 2, ToppingId = 4 },
    new TuberTopping { TuberOrderId = 3, ToppingId = 5 }
];

//add endpoints here

app.Run();
//don't touch or move this!
public partial class Program { }
