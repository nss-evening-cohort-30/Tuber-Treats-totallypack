using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuberTreats.Data;
using TuberTreats.Models;

namespace TuberTreats.Controllers;

[ApiController]
[Route("[controller]")]
public class TuberDriversController : ControllerBase
{
    private readonly TuberTreatsDbContext _context;

    public TuberDriversController(TuberTreatsDbContext context)
    {
        _context = context;
    }

    // GET: tuberdrivers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TuberDriver>>> GetTuberDrivers()
    {
        var drivers = await _context.TuberDrivers
            .OrderBy(d => d.Name)
            .ToListAsync();

        return Ok(drivers);
    }

    // GET: tuberdrivers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TuberDriver>> GetTuberDriver(int id)
    {
        var tuberDriver = await _context.TuberDrivers
            .Where(d => d.Id == id)
            .Include(d => d.TuberDeliveries
                .OrderByDescending(o => o.OrderPlacedOnDate))
                .ThenInclude(o => o.Customer)
            .Include(d => d.TuberDeliveries)
                .ThenInclude(o => o.TuberToppings)
                    .ThenInclude(tt => tt.Topping)
            .FirstOrDefaultAsync();

        if (tuberDriver == null)
        {
            return NotFound();
        }

        return Ok(tuberDriver);
    }
}
