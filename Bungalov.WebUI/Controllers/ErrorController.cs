using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace Bungalov.WebUI.Controllers;

public class ErrorController : Controller
{
    public IActionResult Index()
    {
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        
        if (exceptionDetails != null)
        {
            Log.Error($"Hata Yolu: {exceptionDetails.Path}, Hata Mesajı: {exceptionDetails.Error.Message}, StackTrace: {exceptionDetails.Error.StackTrace}");
        }

        return View();
    }

    [Route("Error/NotFound/{statusCode}")]
    public IActionResult NotFound(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        if (statusCodeResult != null)
        {
            Log.Warning($"404 Hatası - Yol: {statusCodeResult.OriginalPath}");
        }
        
        return View();
    }
}
