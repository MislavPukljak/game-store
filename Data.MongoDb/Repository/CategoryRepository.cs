using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly IMongoCollection<Category> _dbSet;

    public CategoryRepository(IMongoContext context)
        : base(context, "categories")
    {
        _dbSet = context.GetCollection<Category>("categories");
    }

    public async Task<Category> GetByCategoryNameAsync(string categoryName, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq("CategoryName", categoryName);

        var categories = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return categories;
    }

    public async Task<Category> GetByCategoryId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq("CategoryID", id);

        var suppliers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return suppliers;
    }

    public async Task DeleteCategoryAsync(string name, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq("CategoryName", name);

        await _dbSet.DeleteOneAsync(filter, cancellationToken);
    }
}
