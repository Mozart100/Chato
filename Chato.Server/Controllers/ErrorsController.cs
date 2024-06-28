//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Mvc;

//namespace Chato.Server.Controllers
//{
//    public class ErrorsController : ControllerBase
//    {
//        [HttpGet]
//        [Route("/error")]
//        public IActionResult Error()
//        {
//            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
//            return Problem(title: exception?.Message);

//        }
//    }
//}
