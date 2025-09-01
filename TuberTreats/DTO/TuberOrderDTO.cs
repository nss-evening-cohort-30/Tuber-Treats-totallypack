namespace TuberTreats.DTO;

public class TuberOrderDTO
{
  public int CustomerId { get; set; }
  public List<int> ToppingId { get; set; } = [];
}
