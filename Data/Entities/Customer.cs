namespace Data.SQL.Entities;

public class Customer
{
    public enum BanDuration
    {
        OneHour,
        OneDay,
        OneWeek,
        OneMonth,
        Forever,
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public List<Order>? Orders { get; set; }

    public bool IsPermanentlyBaned { get; set; }

    public DateTime? BanedUntil { get; set; }
}
