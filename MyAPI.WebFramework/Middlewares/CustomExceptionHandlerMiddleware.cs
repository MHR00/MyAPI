using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyAPI.WebFramework.api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAPI.WebFramework.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next ,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "خطایی رخ  داده است");

                var apiResult = new ApiResult(false , ApiResultStatusCode.ServerError);
                var json = JsonConvert.SerializeObject(apiResult);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
