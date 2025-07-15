using Business.DTO;

namespace Business.Interfaces;

public interface IFileService
{
    Task UpdatePictureToAzureAsync(ImageDataDto imageDto, CancellationToken cancellationToken);

    Task<Stream> GetImageUrlAsync(string fileName);

    Task DeleteImageAsync(string fileName);

    string GetImageUrl(string imageName);
}
