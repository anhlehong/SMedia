using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SMedia.Configuration
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var userId = context.User.Identity.IsAuthenticated 
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown"
                : "Anonymous";
            var token = context.Request.Headers["Authorization"].ToString();
            var method = context.Request.Method;
            var path = context.Request.Path;

            // Log Request
            var requestLog = await FormatRequest(context.Request, userId, token);
            Log.Information("Request: {Request}", requestLog);

            // Lưu trữ response body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            Exception caughtException = null;
            try
            {
                // Gọi middleware tiếp theo
                await _next(context);
            }
            catch (Exception ex)
            {
                caughtException = ex;
                Log.Error("Error: {ErrorMessage}", GetRootErrorMessage(ex));
                context.Response.StatusCode = 500;
            }
            finally
            {
                // Log Response
                var responseLog = await FormatResponse(context.Response, method, path);
                stopwatch.Stop();
                Log.Information("Response (Duration: {Duration}ms): {Response}", 
                    stopwatch.ElapsedMilliseconds, responseLog);

                // Copy response body về stream gốc
                try
                {
                    if (responseBody.CanRead && responseBody.Length > 0)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream, context.RequestAborted);
                    }
                }
                catch (ObjectDisposedException)
                {
                    Log.Warning("Response stream was disposed prematurely.");
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }

                if (caughtException != null)
                {
                    throw caughtException;
                }
            }
        }

        private async Task<string> FormatRequest(HttpRequest request, string userId, string token)
        {
            // Tạo URL đầy đủ
            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            // Lấy path parameters
            var pathParams = request.RouteValues.Any()
                ? string.Join("\n", request.RouteValues.Select(p => $"  {p.Key}: {p.Value}"))
                : null;

            // Lấy query parameters
            var queryParams = request.Query.Any()
                ? string.Join("\n", request.Query.Select(q => $"  {q.Key}: {q.Value}"))
                : null;

            // Đọc request body
            request.EnableBuffering();
            string body = string.Empty;
            if (request.Body.CanRead)
            {
                using var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            // Định dạng JSON nếu là application/json
            if (!string.IsNullOrEmpty(body) && request.ContentType?.Contains("application/json") == true)
            {
                try
                {
                    var json = JsonSerializer.Deserialize<object>(body);
                    body = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
                }
                catch
                {
                    // Giữ nguyên body nếu không parse được
                }
            }

            // Chỉ log thông tin có giá trị
            var logItems = new List<string>
            {
                $"URL: {url}",
                $"Authenticated: {(string.IsNullOrEmpty(userId) ? "No" : "Yes")}"
            };
            if (!string.IsNullOrEmpty(token)) logItems.Add($"Token: {token}");
            if (pathParams != null) logItems.Add($"Path Parameters:\n{pathParams}");
            if (queryParams != null) logItems.Add($"Query Parameters:\n{queryParams}");
            if (!string.IsNullOrEmpty(body)) logItems.Add($"Body:\n{body}");

            return string.Join("\n", logItems);
        }

        private async Task<string> FormatResponse(HttpResponse response, string method, string path)
        {
            // Đọc response body
            string body = string.Empty;
            if (response.Body.CanRead && response.Body.Length > 0)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                body = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            // Định dạng JSON nếu là application/json
            if (!string.IsNullOrEmpty(body) && response.ContentType?.Contains("application/json") == true)
            {
                try
                {
                    var json = JsonSerializer.Deserialize<object>(body);
                    body = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
                }
                catch
                {
                    // Giữ nguyên body nếu không parse được
                }
            }

            // Log Method, Path, StatusCode và Body (nếu có)
            var logItems = new List<string>
            {
                $"API: {method} {path}",
                $"StatusCode: {response.StatusCode}"
            };
            if (!string.IsNullOrEmpty(body)) logItems.Add($"Body:\n{body}");

            return string.Join("\n", logItems);
        }

        private string GetRootErrorMessage(Exception ex)
        {
            // Lấy thông báo lỗi chính, bỏ "The statement has been terminated"
            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbEx && dbEx.InnerException != null)
            {
                var message = dbEx.InnerException.Message;
                return message.Replace("The statement has been terminated.", "").Trim();
            }
            return ex.Message;
        }
    }
}