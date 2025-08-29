using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TuberTreats.Models;

public class TuberTopping
{
    public int Id { get; set; }

    [Required]
    public int TuberOrderId { get; set; }

    [Required]
    public int ToppingId { get; set; }

    // Navigation properties - can be null when not loaded
    public TuberOrder? TuberOrder { get; set; }
    public Topping? Topping { get; set; }
}
