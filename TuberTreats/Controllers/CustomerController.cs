// CustomersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using TuberTreats.Models;
using TuberTreats.DTOs;

namespace TuberTreats.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly TuberTreatsDbContext _context;

    public CustomersController(TuberTreatsDbContext context)
    {
        _context = context;
    }

    // GET: customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _context.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(customers);
    }

    // GET: customers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == id)
            .Include(c => c.TuberOrders
                .OrderByDescending(o => o.OrderPlacedOnDate))
                .ThenInclude(o => o.TuberDriver)
            .Include(c => c.TuberOrders)
                .ThenInclude(o => o.TuberToppings)
                    .ThenInclude(tt => tt.Topping)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    // POST: customers - Handle Customer object from tests
    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customerRequest)
    {
        if (string.IsNullOrWhiteSpace(customerRequest.Name))
        {
            return BadRequest("Customer name is required");
        }

        if (string.IsNullOrWhiteSpace(customerRequest.Address))
        {
            return BadRequest("Customer address is required");
        }

        var existingCustomer = await _context.Customers
            .Where(c => c.Name.ToLower() == customerRequest.Name.ToLower())
            .FirstOrDefaultAsync();

        if (existingCustomer != null)
        {
            return BadRequest("Customer with this name already exists");
        }

        var customer = new Customer
        {
            Name = customerRequest.Name,
            Address = customerRequest.Address
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
    }

    // DELETE: customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == id)
            .Include(c => c.TuberOrders)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            return NotFound();
        }

        if (customer.TuberOrders.Any())
        {
            return BadRequest("Cannot delete customer with existing orders. Please cancel or complete all orders first.");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
