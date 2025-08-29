using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using TuberTreats.Models;

namespace TuberTreats.Controllers;

[ApiController]
[Route("[controller]")]
public class ToppingsController : ControllerBase
{
    private readonly TuberTreatsDbContext _context;

    public ToppingsController(TuberTreatsDbContext context)
    {
        _context = context;
    }

    // GET: toppings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Topping>>> GetToppings()
    {
        var toppings = await _context.Toppings.ToListAsync();
        return Ok(toppings);
    }

    // GET: toppings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Topping>> GetTopping(int id)
    {
        var topping = await _context.Toppings.FindAsync(id);

        if (topping == null)
        {
            return NotFound();
        }

        return Ok(topping);
    }
}
