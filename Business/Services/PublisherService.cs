using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class PublisherService : IPublisherService
{
    private const string AllPublisher = "allPublishers";

    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<PublisherService> logger, INoSqlUnitOfWork noSqlUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _logger = logger;
        _noSqlUnitOfWork = noSqlUnitOfWork;
    }

    public async Task AddAsync(PublisherDto publisherDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding a new publisher to the database");

        var publisher = _mapper.Map<Publisher>(publisherDto);

        await _unitOfWork.PublisherRepository.AddAsync(publisher, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Added a new publisher to the database");
    }

    public async Task DeleteAsync(string companyName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting publisher with CompanyName: {name}", companyName);

        var publisher = await _unitOfWork.PublisherRepository.GetPublisherByCompayName(companyName, cancellationToken);
        if (publisher is not null)
        {
            await DeleteSqlAsync(companyName, cancellationToken);
            return;
        }

        var supplier = await _noSqlUnitOfWork.SupplierRepository.GetByCompanyNameAsync(companyName, cancellationToken);
        if (supplier is not null)
        {
            await DeleteMongoAsync(companyName, cancellationToken);
            return;
        }

        _logger.LogError("Publisher hasn't been found in db.");

        throw new PublisherException($"Publisher with CompanyName {companyName} not found");
    }

    public async Task<PublisherWithCountDto> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all publishers from the database");

        var cachedPublisher = _memoryCache.Get<IEnumerable<PublisherDto>>(AllPublisher);

        if (cachedPublisher is not null)
        {
            var cachedPublisherCount = new PublisherWithCountDto
            {
                TotalCount = cachedPublisher.Count(),
                Publishers = cachedPublisher,
            };

            return cachedPublisherCount;
        }

        var publishers = await _unitOfWork.PublisherRepository.GetAllAsync(cancellationToken);
        var suppliers = await _noSqlUnitOfWork.SupplierRepository.GetAllAsync(cancellationToken);

        var publisherDto = _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        var supplierDto = _mapper.Map<IEnumerable<PublisherDto>>(suppliers);

        var publishersSuppliers = publisherDto.Concat(supplierDto);

        var publisherWithCountDto = new PublisherWithCountDto
        {
            TotalCount = publishersSuppliers.Count(),
            Publishers = publishersSuppliers,
        };

        _memoryCache.Set(AllPublisher, publisherDto, TimeSpan.FromSeconds(3));

        return publisherWithCountDto;
    }

    public async Task<PublisherDto> GetByCompanyName(string companyName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting publisher by id from the database");

        var cachedPublisher = _memoryCache.Get<PublisherDto>(companyName);

        if (cachedPublisher is not null)
        {
            return cachedPublisher;
        }

        var publisher = await _unitOfWork.PublisherRepository.GetPublisherByCompayName(companyName, cancellationToken);
        var supplier = await _noSqlUnitOfWork.SupplierRepository.GetByCompanyNameAsync(companyName, cancellationToken);

        if (publisher is not null)
        {
            var publisherEntity = _mapper.Map<PublisherDto>(publisher);

            _memoryCache.Set(companyName, publisherEntity, TimeSpan.FromSeconds(3));

            return publisherEntity;
        }
        else if (supplier is not null)
        {
            var supplierEntity = _mapper.Map<PublisherDto>(supplier);

            return supplierEntity;
        }

        _logger.LogError("Publisher hasn't been found in db.");

        throw new PublisherException($"Publisher with name {companyName} not found");
    }

    public async Task<PublisherDto> GetPublisherByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting publisher by id {id} from the database", id);

        var publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(id, cancellationToken);

        var supplier = await _noSqlUnitOfWork.SupplierRepository.GetBySupplierId(id, cancellationToken);

        if (publisher is not null)
        {
            var publisherEntity = _mapper.Map<PublisherDto>(publisher);

            return publisherEntity;
        }
        else if (supplier is not null)
        {
            var supplierEntity = _mapper.Map<PublisherDto>(supplier);

            return supplierEntity;
        }

        _logger.LogError("Game with alias: {id} does not exists.", id);

        throw new GameException("Game not found");
    }

    public async Task<PublisherDto> GetPublisherByGameKey(string key, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting publisher from the database by game key.");

            var games = await _unitOfWork.PublisherRepository.GetPublisherByGameKey(key, CancellationToken.None);

            if (games is not null)
            {
                var gameModels = _mapper.Map<PublisherDto>(games);

                return gameModels;
            }

            throw new GameException("Games not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Game not found {ex}", ex);
        }

        throw new GameException("Games not found.");
    }

    public async Task<PublisherDto> UpdateAsync(PublisherDto publisherDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating publisher by id from the database");

        var existingPublisher = await UpdateExistAsync(publisherDto, cancellationToken);
        if (existingPublisher is not null)
        {
            return existingPublisher;
        }

        var fromMongo = await UpdateNewFromMongoAsync(publisherDto, cancellationToken);

        if (fromMongo is not null)
        {
            return fromMongo;
        }

        _logger.LogError("Publisher hasn't been found in db.");

        throw new PublisherException($"Publisher with id {publisherDto.Id} not found");
    }

    private async Task<PublisherDto> UpdateExistAsync(PublisherDto publisherDto, CancellationToken cancellationToken)
    {
        var publisher = await _unitOfWork.PublisherRepository.GetPublisherByCompayName(publisherDto.CompanyName, cancellationToken);

        if (publisher is not null)
        {
            publisherDto.Id = publisher.Id;
            var publisherModel = _mapper.Map<Publisher>(publisherDto);

            _unitOfWork.PublisherRepository.Update(publisherModel);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Publisher with id {Id} updated", publisher.Id);

            return _mapper.Map<PublisherDto>(publisherDto);
        }

        return null;
    }

    private async Task<PublisherDto> UpdateNewFromMongoAsync(PublisherDto publisherDto, CancellationToken cancellationToken)
    {
        var supplier = await _noSqlUnitOfWork.SupplierRepository.GetByCompanyNameAsync(publisherDto.CompanyName, cancellationToken);

        if (supplier is not null)
        {
            var publisherModel = _mapper.Map<Publisher>(publisherDto);

            await _unitOfWork.PublisherRepository.AddAsync(publisherModel, cancellationToken);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Supplier: {supplier} updated", supplier);

            return _mapper.Map<PublisherDto>(publisherDto);
        }

        return null;
    }

    private async Task DeleteSqlAsync(string companyName, CancellationToken cancellationToken)
    {
        var publisher = await _unitOfWork.PublisherRepository.GetPublisherByCompayName(companyName, cancellationToken);

        _unitOfWork.PublisherRepository.DeleteAsync(publisher, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Publisher: {publisher} deleted", publisher);
    }

    private async Task DeleteMongoAsync(string companyName, CancellationToken cancellationToken)
    {
        var supplier = await _noSqlUnitOfWork.SupplierRepository.GetByCompanyNameAsync(companyName, cancellationToken);

        await _noSqlUnitOfWork.SupplierRepository.DeleteSupplierAsync(supplier.Id, cancellationToken);

        await _noSqlUnitOfWork.Save();

        _logger.LogInformation("Supplier: {supplier} deleted", supplier);
    }
}
