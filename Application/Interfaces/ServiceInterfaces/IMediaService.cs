using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.ServiceInterfaces;

public interface IMediaService
{
    Task<(string MediaUrl, string MediaType)> UploadMediaAsync(IFormFile file, string userId);
    Task DeleteMediaAsync(string blobName);
}
