namespace Business.DTO;

public class CreateGameDto
{
    public CreateModelGameDto Game { get; set; }

    public List<int> Genres { get; set; }

    public List<int> Platforms { get; set; }

    public int Publishers { get; set; }
}
