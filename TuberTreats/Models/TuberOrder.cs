namespace TuberTreats.Models;

public class TuberOrder
{
    public int Id { get; set; }
    public DateTime OrderPlacedOnDate { get; set; }
    public int CustomerId { get; set; }
    public int? TuberDriverId { get; set; }
    public DateTime? DeliveredOnDate { get; set; }

    public Customer Customer { get; set; }
    public TuberDriver TuberDriver { get; set; }
    public List<TuberTopping> TuberToppings { get; set; } = [];

    // THIS is where the Toppings property should be:
    private List<Topping> _toppings = null;

    public List<Topping> Toppings
    {
        get
        {
            if (_toppings != null)
            {
                return _toppings;
            }

            if (TuberToppings == null || TuberToppings.Count == 0)
            {
                _toppings = [];
                return _toppings;
            }

            _toppings = [.. TuberToppings
                .Where(tt => tt.Topping != null)
                .Select(tt => tt.Topping!)];

            return _toppings;
        }
        set
        {
            _toppings = value;
        }
    }
}
