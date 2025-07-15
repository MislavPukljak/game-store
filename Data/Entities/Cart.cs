namespace Data.SQL.Entities;

public class Cart
{
    public int Id { get; set; }

    public ICollection<CartItem> CartItems { get; set; }
}
