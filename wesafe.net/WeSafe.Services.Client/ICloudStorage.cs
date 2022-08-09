using System;
using System.IO;
using System.Threading.Tasks;

namespace WeSafe.Services.Client
{
    public interface ICloudStorage : IFileStorage
    {
        Task<string> CreateFileAsync(string fileName, string contentType, Stream stream);

        DateTimeOffset? GetExpirationTime();
    }
}