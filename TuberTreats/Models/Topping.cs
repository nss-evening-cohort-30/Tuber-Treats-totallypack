namespace TuberTreats.Models;

public class Topping
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<TuberTopping> TuberToppings { get; set; } = [];
}
