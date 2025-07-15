namespace Business.DTO;

public class OrderDetailDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public GameDto ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }
}