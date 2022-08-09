using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Nano.Services.Stubs
{
    public class StubGoogleCloudStorage : PhysicalFileStorage, ICloudStorage
    {
        public StubGoogleCloudStorage(IOptions<PhysicalFileStorageOptions> options) : base(options)
        {
        }

        public async Task<string> CreateFileAsync(string fileName, string contentType, Stream stream)
        {
            await CreateFileAsync(fileName, stream);

            return GetFileUrl(fileName);
        }

        public DateTimeOffset? GetExpirationTime()
        {
            return null;
        }
    }
}