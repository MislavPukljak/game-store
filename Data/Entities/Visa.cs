namespace Data.SQL.Entities;

public class Visa
{
    public int Id { get; set; }

    public string CardHolderName { get; set; }

    public string CardNumber { get; set; }

    public DateTime DateOfExpiry { get; set; }

    public int CVV2 { get; set; }
}
