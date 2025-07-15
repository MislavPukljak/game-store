namespace Data.SQL.Entities;

public class Comment
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public int? ParentId { get; set; }

    public Comment ParentComment { get; set; }

    public List<Comment> ChildComments { get; set; }

    public int GameId { get; set; }

    public Game Game { get; set; }
}
