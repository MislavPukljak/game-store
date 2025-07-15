using Data.SQL.Entities;

namespace Business.DTO;

public class OrderDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public List<OrderDetailDto> OrderDetails { get; set; }

    public decimal Sum { get; set; }

    public DateTime CreationDate { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime PaidOrder { get; set; }
}