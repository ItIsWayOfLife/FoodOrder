using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class ExceptionInterceptor
    {
        public readonly RequestDelegate _next;
        private readonly ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(RequestDelegate next,
           ILogger<ExceptionInterceptor> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Something went wrong: {ex}");
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;

            string result = JsonConvert.SerializeObject(new { error = ex.Message });

            context.Response.ContentType = "aplication/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
