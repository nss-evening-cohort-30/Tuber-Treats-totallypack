using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TuberTreats.Models;

public class TuberDriver
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Navigation property - always initialize to prevent null reference exceptions
    public List<TuberOrder> TuberDeliveries { get; set; } = new List<TuberOrder>();
}
