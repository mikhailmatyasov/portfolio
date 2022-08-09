#if NET452
namespace System.Net.Http
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string name = null);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name = null)
        {
            return new HttpClient();
        }
    }
}
#endif
