using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using TuberTreats.Models;
using TuberTreats.DTOs;

namespace TuberTreats.Controllers;

[ApiController]
[Route("[controller]")]
public class TuberToppingsController : ControllerBase
{
    private readonly TuberTreatsDbContext _context;

    public TuberToppingsController(TuberTreatsDbContext context)
    {
        _context = context;
    }

    // GET: tubertoppings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TuberTopping>>> GetTuberToppings()
    {
        var tuberToppings = await _context.TuberToppings
            .Include(tt => tt.TuberOrder)
            .Include(tt => tt.Topping)
            .ToListAsync();

        return Ok(tuberToppings);
    }

    // GET: tubertoppings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TuberTopping>> GetTuberTopping(int id)
    {
        var tuberTopping = await _context.TuberToppings
            .Include(tt => tt.TuberOrder)
            .Include(tt => tt.Topping)
            .FirstOrDefaultAsync(tt => tt.Id == id);

        if (tuberTopping == null)
        {
            return NotFound();
        }

        return Ok(tuberTopping);
    }

    // POST: tubertoppings - Handle TuberTopping object from tests
    [HttpPost]
    public async Task<ActionResult<TuberTopping>> AddToppingToOrder([FromBody] TuberTopping tuberToppingRequest)
    {
        var tuberTopping = new TuberTopping
        {
            TuberOrderId = tuberToppingRequest.TuberOrderId,
            ToppingId = tuberToppingRequest.ToppingId
        };

        _context.TuberToppings.Add(tuberTopping);
        await _context.SaveChangesAsync();

        var createdTuberTopping = await _context.TuberToppings
            .Include(tt => tt.TuberOrder)  // Add this back
            .Include(tt => tt.Topping)
            .FirstAsync(tt => tt.Id == tuberTopping.Id);

        return CreatedAtAction("GetTuberTopping", new { id = tuberTopping.Id }, createdTuberTopping);  // Change back to CreatedAtAction
    }

    // DELETE: tubertoppings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveToppingFromOrder(int id)
    {
        var tuberTopping = await _context.TuberToppings.FindAsync(id);
        if (tuberTopping == null)
        {
            return NotFound();
        }

        _context.TuberToppings.Remove(tuberTopping);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
