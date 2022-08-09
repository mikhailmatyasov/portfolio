using Model.Dto.Login;
using System.Threading.Tasks;

namespace ScheduleService.Services.Login
{
    public interface ILoginService
    {
        /// <summary>
        /// Validates account credentials by login and password
        /// </summary>
        /// <param name="login">Account login.</param>
        /// <param name="password">Account password.</param>
        /// <returns>Authentication result.</returns>
        Task<LoginResultDto> LoginAsync(string login, string password);

        /// <summary>
        /// Returns forgot password url
        /// </summary>
        /// <returns>Forgot password url.</returns>
        string GetForgotPasswordUrl();
    }
}
