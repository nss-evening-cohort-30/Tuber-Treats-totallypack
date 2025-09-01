using TuberTreats.Models;

namespace TuberTreats.DTO;

public class TuberOrderDTO
{
  public int CustomerId { get; set; }
  public List<int> ToppingId { get; set; } = [];
}

// DTO for creating a new order (request body)
public class CreateOrderRequest
{
  public int WheelId { get; set; }
  public int TechnologyId { get; set; }
  public int PaintId { get; set; }
  public int InteriorId { get; set; }
}
