namespace Business.DTO;

public class CommentDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public int? ParentId { get; set; }

    public List<CommentDto>? ChildComments { get; set; }
}
