using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByGameAlias(string key, CancellationToken cancellationToken);

    Task<Comment> GetCommentById(int id, CancellationToken cancellationToken);

    Task AddCommentToGameAsync(Comment comment, CancellationToken cancellationToken);

    Task<Comment> RemoveComment(int id, CancellationToken cancellationToken);
}
