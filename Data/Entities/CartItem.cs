﻿namespace Data.SQL.Entities;

public class CartItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }

    public List<Game> Products { get; set; }
}