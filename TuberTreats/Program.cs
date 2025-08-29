using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Fix circular reference issues
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with InMemory database for testing
builder.Services.AddDbContext<TuberTreatsDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use InMemory database for development and testing
        options.UseInMemoryDatabase("TuberTreatsDb");
    }
    else
    {
        // Use SQL Server for production (if needed)
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TuberTreatsDbContext>();
    SeedData(context);
}

app.Run();

static void SeedData(TuberTreatsDbContext context)
{
    // Only seed if database is empty
    if (context.Customers.Any())
        return;

    // Seed data here - this will be called once when app starts
    context.Database.EnsureCreated();
    
    // Add seed data (same as before)
    var customers = new[]
    {
        new TuberTreats.Models.Customer { Name = "Alice Johnson", Address = "123 Main St, Anytown, USA" },
        new TuberTreats.Models.Customer { Name = "Bob Smith", Address = "456 Oak Ave, Somewhere, USA" },
        new TuberTreats.Models.Customer { Name = "Carol Davis", Address = "789 Pine Rd, Elsewhere, USA" },
        new TuberTreats.Models.Customer { Name = "Dave Wilson", Address = "321 Elm St, Nowhere, USA" },
        new TuberTreats.Models.Customer { Name = "Eve Brown", Address = "654 Maple Dr, Anywhere, USA" }
    };
    context.Customers.AddRange(customers);
    context.SaveChanges();

    var drivers = new[]
    {
        new TuberTreats.Models.TuberDriver { Name = "Frank Miller" },
        new TuberTreats.Models.TuberDriver { Name = "Grace Lee" },
        new TuberTreats.Models.TuberDriver { Name = "Henry Adams" }
    };
    context.TuberDrivers.AddRange(drivers);
    context.SaveChanges();

    var toppings = new[]
    {
        new TuberTreats.Models.Topping { Name = "Butter" },
        new TuberTreats.Models.Topping { Name = "Sour Cream" },
        new TuberTreats.Models.Topping { Name = "Chives" },
        new TuberTreats.Models.Topping { Name = "Bacon Bits" },
        new TuberTreats.Models.Topping { Name = "Cheese" }
    };
    context.Toppings.AddRange(toppings);
    context.SaveChanges();

    var orders = new[]
    {
        new TuberTreats.Models.TuberOrder { OrderPlacedOnDate = DateTime.Now.AddDays(-2), CustomerId = 1, TuberDriverId = 1 },
        new TuberTreats.Models.TuberOrder { OrderPlacedOnDate = DateTime.Now.AddDays(-1), CustomerId = 2, TuberDriverId = 2, DeliveredOnDate = DateTime.Now.AddHours(-2) },
        new TuberTreats.Models.TuberOrder { OrderPlacedOnDate = DateTime.Now.AddHours(-3), CustomerId = 3 }
    };
    context.TuberOrders.AddRange(orders);
    context.SaveChanges();

    var tuberToppings = new[]
    {
        new TuberTreats.Models.TuberTopping { TuberOrderId = 1, ToppingId = 1 },
        new TuberTreats.Models.TuberTopping { TuberOrderId = 1, ToppingId = 2 },
        new TuberTreats.Models.TuberTopping { TuberOrderId = 2, ToppingId = 3 },
        new TuberTreats.Models.TuberTopping { TuberOrderId = 2, ToppingId = 4 },
        new TuberTreats.Models.TuberTopping { TuberOrderId = 3, ToppingId = 5 }
    };
    context.TuberToppings.AddRange(tuberToppings);
    context.SaveChanges();
}

// Make the implicit Program class public for testing
public partial class Program { }
