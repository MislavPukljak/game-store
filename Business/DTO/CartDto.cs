namespace Business.DTO;

public class CartDto
{
    public int Id { get; set; }

    public List<CartItemsDto> CartItems { get; set; }
}
