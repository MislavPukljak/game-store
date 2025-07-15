using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IImagesDataRepository : IGenericRepository<ImagesData>
{
    Task<ImagesData> GetByNameAsync(string name);
}
