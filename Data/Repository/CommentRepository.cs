using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private const string _comment = "A comment/quote was deleted";
    private readonly GameStoreContext _context;

    public CommentRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByGameAlias(string key, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .AsNoTracking()
            .Where(x => x.Game.Alias == key)
            .ToListAsync(cancellationToken);

        return comments;
    }

    public async Task<Comment> GetCommentById(int id, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .AsNoTracking()
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return comment;
    }

    public async Task AddCommentToGameAsync(Comment comment, CancellationToken cancellationToken)
    {
        await _context.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task<Comment> RemoveComment(int id, CancellationToken cancellationToken)
    {
        var comment = await LoadCommentWithChildren(id, cancellationToken);

        if (comment == null)
        {
            return null;
        }

        UpdateCommentAndChildComments(comment);

        _context.Comments.Update(comment);

        await _context.SaveChangesAsync(cancellationToken);

        return comment;
    }

    private static void UpdateCommentAndChildComments(Comment comment)
    {
        comment.Body = _comment;

        if (comment.ChildComments != null)
        {
            foreach (var childComment in comment.ChildComments)
            {
                UpdateCommentAndChildComments(childComment);
            }
        }
    }

    private async Task<Comment> LoadCommentWithChildren(int id, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

        if (comment != null)
        {
            await _context.Entry(comment)
                .Collection(c => c.ChildComments)
                .LoadAsync(cancellationToken);

            if (comment.ChildComments != null)
            {
                // Load the children (and their children, etc.) for each child comment
                foreach (var childComment in comment.ChildComments)
                {
                    await LoadCommentWithChildren(childComment.Id, cancellationToken);
                }
            }
        }

        return comment;
    }
}
