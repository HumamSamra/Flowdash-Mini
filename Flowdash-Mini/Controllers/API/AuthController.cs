using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.Controllers.API
{
    [Route("API/[controller]"), ApiController]
    public class AuthController : Controller
    {
        [HttpGet("TestEndpoint/{text}")]
        public ActionResult TestEndpoint(string text)
        {
            return Ok($"Test was successful: {text}");
        }
    }
}
