using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface IAuthTokenGenerator
    {
        TokenResponse CreateToken(TokenRequest request);
    }
}