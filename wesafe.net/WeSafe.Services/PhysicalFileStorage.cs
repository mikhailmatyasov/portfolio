using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class PhysicalFileStorage : IFileStorage
    {
        private readonly PhysicalFileStorageOptions _options;

        public PhysicalFileStorage(IOptions<PhysicalFileStorageOptions> options)
        {
            _options = options.Value;

            _options.BaseUrl = _options.BaseUrl.TrimEnd('/', '\\');
            _options.RequestPath = _options.RequestPath.TrimEnd('/', '\\').TrimStart('/', '\\');
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
            return $"{_options.BaseUrl}/{_options.RequestPath}/{fileName}";
        }

        public string GetFilePath(string fileUrl)
        {
            string url = $"{_options.BaseUrl}/{_options.RequestPath}/";

            if ( fileUrl.StartsWith(url) )
                fileUrl = fileUrl.Substring(url.Length);

            return $"{_options.Root}/{fileUrl}";
        }

        public void DeleteFile(string fileName)
        {
            if ( !String.IsNullOrEmpty(fileName) )
            {
                File.Delete(GetFilePath(fileName));
            }
        }

        public async Task CreateFileAsync(string fileName, Stream content)
        {
            using ( var stream = CreateStream(fileName) )
            {
                await content.CopyToAsync(stream);
            }
        }

        private Stream CreateStream(string fileName)
        {
            return File.Create($"{_options.Root}/{fileName}");
        }
    }
}