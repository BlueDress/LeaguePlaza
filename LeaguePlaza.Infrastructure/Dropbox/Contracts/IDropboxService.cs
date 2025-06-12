using Microsoft.AspNetCore.Http;

namespace LeaguePlaza.Infrastructure.Dropbox.Contracts
{
    public interface IDropboxService
    {
        Task<string> GetAccessToken();
        Task<string> UploadImage(IFormFile image, string path, string accessToken);
    }
}
