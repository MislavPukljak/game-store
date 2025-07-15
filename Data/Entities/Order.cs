namespace Data.SQL.Entities;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; }

    public DateTime OrderDate { get; set; }

    public List<OrderDetail> OrderDetails { get; set; }

    public decimal Sum { get; set; }

    public DateTime CreationDate { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime PaidOrder { get; set; }
}
