using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeSafe.Web.Common.Exceptions;
using WeSafe.Web.Common.Exceptions.Models;

namespace WeSafe.Web.Common
{
    /// <summary>
    /// Provides a base http client to communicate between WeSafe microservices
    /// </summary>
    public abstract class WeSafeHttpClient
    {
        #region Fields

        private const string MediaType = "application/json";

        #endregion

        #region Constructors

        protected WeSafeHttpClient(HttpClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        #endregion

        #region Protected properties

        protected HttpClient Client { get; }

        protected string AuthToken { get; set; }

        #endregion

        #region Protected members

        protected async Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            return await SendRequest<T>(request);
        }

        protected async Task<T> Post<T>(string url, object content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaType)
            };

            return await SendRequest<T>(request);
        }

        #endregion

        #region Private members

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            if ( !String.IsNullOrWhiteSpace(AuthToken) )
            {
                request.Headers.Add("Authorization", $"Bearer {AuthToken}");
            }

            var result = await Client.SendAsync(request);

            await EnsureSuccessStatusCode(result);

            var response = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(response);
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage result)
        {
            if ( result.IsSuccessStatusCode ) return;

            var response = await result.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ErrorModel>(response);

            switch ( error.Code )
            {
                case 400:
                    throw new BadRequestException(error.ErrorMessage);
                case 404:
                    throw new NotFoundException(error.ErrorMessage);
                case 500:
                    throw new SystemOperationException(error.ErrorMessage);
                case 401:
                    throw new UnauthorizedException(error.ErrorMessage);
                default:
                    throw new HttpRequestException(error.ErrorMessage);
            }
        }

        #endregion
    }
}