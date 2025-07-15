namespace Business.DTO;

public class CustomerDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public bool IsPermanentlyBaned { get; set; }

    public DateTime? BanedUntil { get; set; }
}
