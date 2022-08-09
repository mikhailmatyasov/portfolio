using System;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCloudStorageOptions _options;
        private readonly UrlSigner _urlSigner;
        private readonly TimeSpan _defaultExpirationTimeSpan = TimeSpan.FromDays(365);
        private readonly StorageClient _storage;

        public GoogleCloudStorage(IOptions<GoogleCloudStorageOptions> options)
        {
            _options = options.Value;
            _urlSigner = UrlSigner.FromServiceAccountPath(_options.GoogleCredentialFile);
            _storage = StorageClient.Create();
        }

        public string GetRandomFileName(string extension = null)
        {
            var random = new Random();
            var sfx = random.Next(1000000, 9999999);
            var fileName = $"{DateTime.UtcNow.Ticks}_{sfx}";

            if ( extension != null ) fileName += $".{extension}";

            return fileName;
        }

        public string GetFileUrl(string fileName)
        {
            string url = _urlSigner.Sign(_options.Bucket, fileName, _defaultExpirationTimeSpan, signingVersion: SigningVersion.V2);

            return url;
        }

        public string GetFilePath(string fileUrl)
        {
            return fileUrl;
        }

        public void DeleteFile(string fileName)
        {
        }

        public async Task CreateFileAsync(string fileName, Stream stream)
        {
            var result = await _storage.UploadObjectAsync(_options.Bucket, fileName, null, stream);
        }

        public async Task<string> CreateFileAsync(string fileName, string contentType, Stream stream)
        {
            var result = await _storage.UploadObjectAsync(_options.Bucket, fileName, contentType, stream);

            return GetFileUrl(fileName);
        }

        public DateTimeOffset? GetExpirationTime()
        {
            return DateTimeOffset.UtcNow + _defaultExpirationTimeSpan;
        }
    }
}