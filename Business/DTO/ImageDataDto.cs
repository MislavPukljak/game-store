using Microsoft.AspNetCore.Http;

namespace Business.DTO;

public class ImageDataDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Container { get; set; }

    public string ContentType { get; set; }

    public IFormFile ImageFile { get; set; }
}
