using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TuberTreats.Models;

public class Customer
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
  public List<TuberOrder> TuberOrders { get; set; } = [];
}
