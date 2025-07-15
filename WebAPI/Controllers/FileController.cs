using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/file")]
[Authorize(Roles = "Administrator, Manager")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] ImageDataDto image, CancellationToken cancellationToken = default)
    {
        await _fileService.UpdatePictureToAzureAsync(image, cancellationToken);

        return Ok();
    }

    [HttpGet("getImage")]
    public async Task<IActionResult> GetImage([FromQuery] string fileName)
    {
        var stream = await _fileService.GetImageUrlAsync(fileName);

        return File(stream, "image/jpeg");
    }

    [HttpGet("downloadImage")]
    public async Task<IActionResult> Download([FromQuery] string fileName)
    {
        var stream = await _fileService.GetImageUrlAsync(fileName);

        return File(stream, "image/jpeg", $"blobfile.jpeg");
    }

    [HttpDelete("deleteImage")]
    public async Task<IActionResult> DeleteImage([FromQuery] string fileName)
    {
        await _fileService.DeleteImageAsync(fileName);

        return Ok();
    }
}
