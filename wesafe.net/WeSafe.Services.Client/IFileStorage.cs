using System.IO;
using System.Threading.Tasks;

namespace WeSafe.Services.Client
{
    public interface IFileStorage
    {
        string GetRandomFileName(string extension = null);

        string GetFileUrl(string fileName);

        string GetFilePath(string fileUrl);

        void DeleteFile(string fileName);

        Task CreateFileAsync(string fileName, Stream stream);
    }
}