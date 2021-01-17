using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class AuthTokenInterceptor
    {
        private const string _authorizationHeader = "Authorization";
        private const string _authorizationScheme = "Bearer";
        private readonly RequestDelegate _next;

        public AuthTokenInterceptor(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var result = context.Request.Headers.TryGetValue(_authorizationHeader, out StringValues tokenValues);
            if (result && !string.IsNullOrEmpty(tokenValues))
            {
                var token = tokenValues.First();
                if (!token.Contains($"{_authorizationScheme} "))
                {
                    token = $"{_authorizationScheme} {token}";
                }

                result = context.Request.Headers.Remove(_authorizationHeader);
                if (result)
                {
                    context.Request.Headers.Add(_authorizationHeader, token);
                }
            }

            await _next.Invoke(context);
        }
    }
}
