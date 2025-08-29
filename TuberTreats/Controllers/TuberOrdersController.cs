using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using TuberTreats.Models;
using TuberTreats.DTOs;

namespace TuberTreats.Controllers;

[ApiController]
[Route("[controller]")]
public class TuberOrdersController : ControllerBase
{
    private readonly TuberTreatsDbContext _context;

    public TuberOrdersController(TuberTreatsDbContext context)
    {
        _context = context;
    }

    // GET: tuberorders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TuberOrder>>> GetTuberOrders()
    {
        var orders = await _context.TuberOrders
            .Include(o => o.Customer)
            .Include(o => o.TuberDriver)
            .Include(o => o.TuberToppings)
                .ThenInclude(tt => tt.Topping)
            .OrderBy(o => o.OrderPlacedOnDate)
            .ToListAsync();

        return Ok(orders);
    }

    // GET: tuberorders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TuberOrder>> GetTuberOrder(int id)
    {
        var tuberOrder = await _context.TuberOrders
            .Where(o => o.Id == id)
            .Include(o => o.Customer)
            .Include(o => o.TuberDriver)
            .Include(o => o.TuberToppings)
                .ThenInclude(tt => tt.Topping)
            .FirstOrDefaultAsync();

        if (tuberOrder == null)
        {
            return NotFound();
        }

        // Force the Toppings property to be evaluated
        var toppingsCount = tuberOrder.Toppings.Count;

        return Ok(tuberOrder);
    }

    // POST: tuberorders - Handle TuberOrder object from tests
    [HttpPost]
    public async Task<ActionResult<TuberOrder>> PostTuberOrder([FromBody] TuberOrder orderRequest)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == orderRequest.CustomerId);
        
        if (!customerExists)
        {
            return BadRequest("Customer not found");
        }

        var tuberOrder = new TuberOrder
        {
            OrderPlacedOnDate = DateTime.Now, // Keep consistent with test expectations
            CustomerId = orderRequest.CustomerId
        };

        _context.TuberOrders.Add(tuberOrder);
        await _context.SaveChangesAsync();

        var createdOrder = await _context.TuberOrders
            .Where(o => o.Id == tuberOrder.Id)
            .Include(o => o.Customer)
            .Include(o => o.TuberDriver)
            .Include(o => o.TuberToppings)
                .ThenInclude(tt => tt.Topping)
            .FirstAsync();

        return CreatedAtAction("GetTuberOrder", new { id = tuberOrder.Id }, createdOrder);
    }

    // PUT: tuberorders/{id} - Handle TuberOrder object from tests
    [HttpPut("{id}")]
    public async Task<IActionResult> AssignDriver(int id, [FromBody] TuberOrder updatedOrder)
    {
        var tuberOrder = await _context.TuberOrders.FirstOrDefaultAsync(o => o.Id == id);

        if (tuberOrder == null)
        {
            return NotFound();
        }

        // Simply assign whatever value is provided
        tuberOrder.TuberDriverId = updatedOrder.TuberDriverId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: tuberorders/{id}/complete
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteOrder(int id)
    {
        var tuberOrder = await _context.TuberOrders
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync();

        if (tuberOrder == null)
        {
            return NotFound();
        }

        if (tuberOrder.TuberDriverId == null)
        {
            return BadRequest("Order must have a driver assigned before it can be completed");
        }

        if (tuberOrder.DeliveredOnDate.HasValue)
        {
            return BadRequest("Order is already completed");
        }

        tuberOrder.DeliveredOnDate = DateTime.Now; // Keep consistent with test expectations
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order completed successfully", deliveredAt = tuberOrder.DeliveredOnDate });
    }
}
