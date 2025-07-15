using Business.DTO;

namespace Business.Interfaces;

public interface IPublisherService
{
    Task<PublisherWithCountDto> GetAllAsync(CancellationToken cancellationToken);

    Task<PublisherDto> GetByCompanyName(string companyName, CancellationToken cancellationToken);

    Task AddAsync(PublisherDto publisherDto, CancellationToken cancellationToken);

    Task<PublisherDto> UpdateAsync(PublisherDto publisherDto, CancellationToken cancellationToken);

    Task DeleteAsync(string companyName, CancellationToken cancellationToken);

    Task<PublisherDto> GetPublisherByGameKey(string key, CancellationToken cancellationToken);

    Task<PublisherDto> GetPublisherByIdAsync(int id, CancellationToken cancellationToken);
}
