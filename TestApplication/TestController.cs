using CustomRouteBinderProviderLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TestApplication
{
    public class TestController : ControllerBase
    {
        [Route("default/{code}")]
        [HttpGet]
        public async Task<ActionResult> GetDefault([FromRoute] string code)
        {
            await Task.CompletedTask;

            return Ok(code);
        }

        [Route("unsafe/{code}")]
        [HttpGet]
        public async Task<ActionResult> GetUnsafe([FromRouteUnsafe] string code)
        {
            await Task.CompletedTask;

            return Ok(code);
        }

        [Route("raw/{code}")]
        [HttpGet]
        public async Task<ActionResult> GetRaw([FromRouteRaw] string code)
        {
            await Task.CompletedTask;

            return Ok(code);
        }
    }
}
