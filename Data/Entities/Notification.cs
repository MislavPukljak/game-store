namespace Data.SQL.Entities;

public class Notification
{
    public int Id { get; set; }

    public string Queue { get; set; }

    public bool IsTurnedOff { get; set; }
}
