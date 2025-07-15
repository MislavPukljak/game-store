using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Business.DTO;
using Business.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Business.Services;

public class FileService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;
    private readonly string _containerName;

    public FileService(BlobServiceClient blobServiceClient, IUnitOfWork unitOfWork, IMemoryCache memoryCache, IOptions<ImagesData> options)
    {
        _blobServiceClient = blobServiceClient;
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
        _containerName = options.Value.Container;
    }

    public async Task<Stream> GetImageUrlAsync(string fileName)
    {
        if (_memoryCache.TryGetValue(fileName, out byte[] cachedImage))
        {
            return cachedImage != null ? new MemoryStream(cachedImage) : null;
        }

        var blob = GetBlobClient(fileName);

        var downloadInfo = await blob.DownloadAsync();

        using (var memoryStream = new MemoryStream())
        {
            await downloadInfo.Value.Content.CopyToAsync(memoryStream);
            cachedImage = memoryStream.ToArray();
        }

        _memoryCache.Set(fileName, cachedImage);

        return downloadInfo.Value.Content;
    }

    public async Task UpdatePictureToAzureAsync(ImageDataDto imageDto, CancellationToken cancellationToken)
    {
        var blob = GetBlobClient(imageDto.ImageFile.FileName);

        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/jpeg",
            },
        };

        await blob.UploadAsync(imageDto.ImageFile.OpenReadStream(), blobUploadOptions, cancellationToken);

        var image = new ImagesData
        {
            Name = imageDto.ImageFile.FileName,
            Container = _containerName,
            ContentType = blobUploadOptions.HttpHeaders.ContentType,
        };

        await _unitOfWork.ImagesDataRepository.AddAsync(image, cancellationToken);

        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteImageAsync(string fileName)
    {
        var blob = GetBlobClient(fileName);

        await DeleteGameImageAsync(fileName, CancellationToken.None);

        await blob.DeleteIfExistsAsync();
    }

    public string GetImageUrl(string imageName)
    {
        var blob = GetBlobClient(imageName);
        return blob.Uri.AbsoluteUri;
    }

    private BlobClient GetBlobClient(string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        return container.GetBlobClient(fileName);
    }

    private async Task DeleteGameImageAsync(string fileName, CancellationToken cancellationToken)
    {
        var image = await _unitOfWork.ImagesDataRepository.GetAllAsync(cancellationToken);

        if (!image.Any())
        {
            return;
        }

        var existingImage = image.FirstOrDefault(x => x.Name == fileName);

        if (existingImage is null)
        {
            return;
        }

        _unitOfWork.ImagesDataRepository.DeleteAsync(existingImage, cancellationToken);

        await _unitOfWork.SaveAsync();
    }
}
