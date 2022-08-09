using MediatR;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Commands.Login
{
    public class LoginCommand : LoginModel, IRequest<TokenResponseModel>
    {
        public string Origin { get; set; }
    }
}
