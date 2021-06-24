using Api.Application.Controllers;
using Api.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Application.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class VersionController : MainController
    {
        public VersionController(
            INotifier notifier, 
            IUserService userService) 
            : base(notifier, userService) { }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation("Retorna a versão da api utilizada")]
        public IActionResult GetVersion()
        {
            return Ok(new { Version = "1.0" });
        }
    }
}
