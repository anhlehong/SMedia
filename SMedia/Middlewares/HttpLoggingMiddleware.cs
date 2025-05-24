using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SMedia.Middlewares
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggingMiddleware> _logger;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the request
            await LogRequest(context);

            // Capture the response
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            // Continue with the pipeline
            await _next(context);

            // Log the response
            await LogResponse(context, responseBodyStream, originalBodyStream);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            _logger.LogInformation(
                "HTTP {Method} {Path}{QueryString} received from {IPAddress}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Connection.RemoteIpAddress);

            // Log headers
            foreach (var header in context.Request.Headers)
            {
                _logger.LogDebug("Header: {Key}={Value}", header.Key, header.Value);
            }

            // Log body for POST/PUT/PATCH requests
            if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH")
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);
                
                var body = await reader.ReadToEndAsync();
                
                if (!string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogInformation("Request Body: {Body}", body);
                }

                // Reset the position to allow reading the body in the controller
                context.Request.Body.Position = 0;
            }
        }

        private async Task LogResponse(HttpContext context, MemoryStream responseBodyStream, Stream originalBodyStream)
        {
            // Log the response
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            
            _logger.LogInformation(
                "HTTP {StatusCode} returned for {Method} {Path}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path);

            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                _logger.LogDebug("Response Body: {Body}", responseBody);
            }

            // Copy the response body to the original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}
