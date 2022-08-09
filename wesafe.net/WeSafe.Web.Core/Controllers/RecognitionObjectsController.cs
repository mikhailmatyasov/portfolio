using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class RecognitionObjectsController : ControllerBase
    {
        #region Fields

        private readonly IRecognitionObjectService _recognitionObjectService;

        #endregion

        #region Constructors

        public RecognitionObjectsController(IRecognitionObjectService recognitionObjectService)
        {
            _recognitionObjectService = recognitionObjectService;
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> GetRecognitionObjectsAsync(bool activeOnly = false)
        {
            return Ok(await _recognitionObjectService.GetRecognitionObjectsAsync(activeOnly));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecognitionObjectAsync(int id)
        {
            return Ok(await _recognitionObjectService.GetRecognitionObjectAsync(id));
        }

        [HttpPost]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> CreateRecognitionObjectAsync([FromBody] RecognitionObjectModel model)
        {
            await _recognitionObjectService.CreateRecognitionObjectAsync(model);

            return Ok();
        }

        [HttpPut]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> UpdateRecognitionObjectAsync([FromBody] RecognitionObjectModel model)
        {
            await _recognitionObjectService.UpdateRecognitionObjectAsync(model);

            return Ok();
        }
    }
}