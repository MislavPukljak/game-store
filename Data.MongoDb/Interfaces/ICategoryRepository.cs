using Data.MongoDb.Entities;

namespace Data.MongoDb.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category> GetByCategoryNameAsync(string categoryName, CancellationToken cancellationToken);

    Task<Category> GetByCategoryId(int id, CancellationToken cancellationToken);

    Task DeleteCategoryAsync(string name, CancellationToken cancellationToken);
}
