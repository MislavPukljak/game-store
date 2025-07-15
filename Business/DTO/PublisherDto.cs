using System.ComponentModel.DataAnnotations;

namespace Business.DTO;

public class PublisherDto
{
    [Key]
    public int Id { get; set; }

    public string CompanyName { get; set; }

    public string Description { get; set; }

    public string HomePage { get; set; }
}
