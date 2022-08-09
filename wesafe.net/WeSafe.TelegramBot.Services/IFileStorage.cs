namespace WeSafe.TelegramBot.Services
{
    public interface IFileStorage
    {
        string GetFileUrl(string fileName);

        string GetFilePath(string fileUrl);
    }
}