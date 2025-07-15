namespace Data.SQL.Entities;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public decimal Sum { get; set; }

    public decimal Discount { get; set; }

    public int Quantity { get; set; }

    public Game Product { get; set; }

    public Order Order { get; set; }
}
