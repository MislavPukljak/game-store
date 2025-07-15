using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.MongoDb.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class ShipperService : IShipperService
{
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ShipperService> _logger;

    public ShipperService(INoSqlUnitOfWork noSqlUnitOfWork, IMapper mapper, ILogger<ShipperService> logger)
    {
        _noSqlUnitOfWork = noSqlUnitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ShipperDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var shippers = await _noSqlUnitOfWork.ShipperRepository.GetAllAsync(cancellationToken);

        if (!shippers.Any())
        {
            _logger.LogError("Shippers not found");

            throw new ShipperException("Shippers not found");
        }

        return _mapper.Map<IEnumerable<ShipperDto>>(shippers);
    }

    public async Task<ShipperDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var shipper = await _noSqlUnitOfWork.ShipperRepository.GetByShipperId(id, cancellationToken);

        if (shipper is null)
        {
            _logger.LogError($"Shipper with id: {id} not found");

            throw new ShipperException($"Shipper with id: {id} not found");
        }

        return _mapper.Map<ShipperDto>(shipper);
    }
}
