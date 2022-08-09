using WeSafe.Authentication.WebApi.Models;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Services.Abstract
{
    public interface IAuthTokenGenerator
    {
        TokenResponseModel CreateToken(TokenRequestModel request);
    }
}
