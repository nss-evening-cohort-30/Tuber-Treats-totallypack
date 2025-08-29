using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TuberTreats.Models;

public class Topping
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Navigation property - always initialize to prevent null reference exceptions
    public List<TuberTopping> TuberToppings { get; set; } = new List<TuberTopping>();
}
