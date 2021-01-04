using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Web.Exceptions
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

                await _next.Invoke(context);
            }
        }
    }
}
