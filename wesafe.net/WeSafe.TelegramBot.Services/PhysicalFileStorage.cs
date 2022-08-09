using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WeSafe.TelegramBot.Services
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

        private Stream CreateStream(string fileName)
        {
            return File.Create($"{_options.Root}/{fileName}");
        }
    }
}