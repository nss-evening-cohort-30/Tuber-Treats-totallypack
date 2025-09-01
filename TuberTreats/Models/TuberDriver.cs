namespace TuberTreats.Models;

public class TuberDriver
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TuberOrder> TuberDeliveries { get; set; } = [];
}
