using System.Net;
using Entity.Exeptions.Common;
using Serilog;
using WebCore.Models;

namespace WebCore.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            if (e is ApiExceptionBase apiException)
            {
                context.Response.StatusCode = apiException.StatusCode;
                await context.Response.WriteAsJsonAsync(
                    ResponseModel.ResultFromException(e, (HttpStatusCode)apiException.StatusCode));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(ResponseModel.ResultFromException(e));
                if (context.Response.StatusCode >= 400)
                    Log.Fatal(e,"Exception: " + e.Message);
            }
            Log.Error(e, "Exception: " + e.Message);
        }
    }
}