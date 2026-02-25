using Flowdash_Mini.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Talabat.Controllers
{
    public class ErrorController : _BaseController
    {

        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.Error = exceptionHandlerPathFeature?.Error;
            return View("Error");
        }
    }
}