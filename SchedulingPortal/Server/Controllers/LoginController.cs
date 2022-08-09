using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Login;
using ScheduleService.Services.Login;
using System;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;

        public LoginController(ILogger<LoginController> logger, ILoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
        {
            _logger.LogDebug(nameof(LoginAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.LoginMeasurement);
                LoginResultDto result = await _loginService.LoginAsync(request.UserName, request.Password);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(LoginAsync)} - {e}");
                throw;
            }
        }

        [HttpGet("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("text/plain")]
        public IActionResult GetForgotPasswordUrl()
        {
            _logger.LogDebug(nameof(GetForgotPasswordUrl));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.LoginMeasurement);
                string result = _loginService.GetForgotPasswordUrl();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(GetForgotPasswordUrl)} - {e}");
                throw;
            }
        }
    }
}
