using Business.DTO;

namespace Business.Interfaces;

public interface IShipperService
{
    Task<IEnumerable<ShipperDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ShipperDto> GetByIdAsync(int id, CancellationToken cancellationToken);
}
