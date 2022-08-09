using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Web.Core.Mappers;
using WeSafe.Web.Core.Models;


namespace WeSafe.Web.Controllers
{
    /// <summary>
    /// Represents Unhandled exceptions operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class UnhandledExceptionController : Controller
    {
        private readonly UnhandledExceptionFilterMapper _unhandledExceptionFilterMapper;
        private readonly IUnhandledExceptionsService _unhandledExceptionsService;

        public UnhandledExceptionController(UnhandledExceptionFilterMapper unhandledExceptionFilterMapper, IUnhandledExceptionsService unhandledExceptionsService)
        {
            _unhandledExceptionFilterMapper = unhandledExceptionFilterMapper;
            _unhandledExceptionsService = unhandledExceptionsService;
        }

        /// <summary>
        /// Gets filtered unhandled exceptions.
        /// </summary>
        /// <param name="unhandledExceptionFilter">The unhandled exception filter model.</param>
        /// <returns>The unhandled exception collection.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUnhandledExceptionsAsync([FromQuery] UnhandledExceptionFilter unhandledExceptionFilter)
        {
            var recordQuery = new UnhandledExceptionRecordQuery();

            if (unhandledExceptionFilter != null)
                recordQuery = _unhandledExceptionFilterMapper.ToUnhandledExceptionRecordQuery(unhandledExceptionFilter);

            var result = await _unhandledExceptionsService.GetUnhandledExceptions(recordQuery);

            var unhandledExceptionsResponse = new PageResponse<UnhandledExceptionModel>
            {
                Items = result.Items,
                Total = result.Total
            };

            return Ok(unhandledExceptionsResponse);
        }
    }
}
