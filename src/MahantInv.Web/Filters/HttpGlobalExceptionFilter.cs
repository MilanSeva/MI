using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MahantInv.SharedKernel.Filter
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _env = env;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public void OnException(ExceptionContext context)
        {
            string correlationId = Guid.NewGuid().ToString();
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                $"Correlation Id:{correlationId} => {context.Exception.Message}");

            ProblemDetails problemDetails;
            int statusCode = (int)HttpStatusCode.InternalServerError;

            problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context.HttpContext,
                    statusCode,
                    title: "Sorry, something went wrong.",
                    detail: _env.IsDevelopment() ? $"Correlation Id:{correlationId} => {context.Exception}" : correlationId
                );

            context.Result = new ObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = statusCode;
            context.ExceptionHandled = true;
        }
    }
}
