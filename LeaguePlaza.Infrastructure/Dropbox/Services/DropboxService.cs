﻿using Dropbox.Api.Files;
using Dropbox.Api;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using LeaguePlaza.Infrastructure.Dropbox.Models.ResponseData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

using static LeaguePlaza.Common.Constants.ErrorConstants;

namespace LeaguePlaza.Infrastructure.Dropbox.Services
{
    public class DropboxService(IConfiguration configuration, ILogger<DropboxService> logger) : IDropboxService
    {
        private const string TokenUrl = "https://api.dropboxapi.com/oauth2/token";

        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<DropboxService> _logger = logger;

        public async Task<string> GetAccessToken()
        {
            using var client = new HttpClient();

            var requestBody = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _configuration["Dropbox:RefreshToken"] ?? string.Empty),
                new KeyValuePair<string, string>("client_id", _configuration["Dropbox:AppKey"] ?? string.Empty),
                new KeyValuePair<string, string>("client_secret", _configuration["Dropbox:AppSecret"] ?? string.Empty)
            ]);

            try
            {
                var response = await client.PostAsync(TokenUrl, requestBody);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                return tokenResult?.AccessToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(GetAccessToken));
                _logger.LogError(ErrorMessage, ex.Message);

                return string.Empty;
            }
        }

        public async Task<string> UploadImage(IFormFile image, string path, string accessToken)
        {
            using var dbx = new DropboxClient(accessToken);
            using var memoryStream = new MemoryStream();

            await image.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            try
            {
                var uploadResponse = await dbx.Files.UploadAsync(path: path, mode: WriteMode.Overwrite.Instance, body: memoryStream);
                var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(path);

                return sharedLink.Url.Replace("dl=0", "raw=1");
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(UploadImage));
                _logger.LogError(ErrorMessage, ex.Message);

                return string.Empty;
            }
        }
    }
}
