using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebHookHub.Models.DB;
using WebHookHub.Services;

namespace WebHookHub.Middleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ApiLogService _apiLogService;

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApiLogService apiLogService)
        {
            try
            {
                _apiLogService = apiLogService;

                var request = httpContext.Request;
                if (request.Path.StartsWithSegments(new PathString("/api")))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.UtcNow;
                    var requestBodyContent = await ReadRequestBody(request);
                    var originalBodyStream = httpContext.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);

                        SafeLog(requestTime,
                            stopWatch.ElapsedMilliseconds,
                            response.StatusCode,
                            request.Method,
                            request.Path,
                            request.QueryString.ToString(),
                            requestBodyContent,
                            responseBodyContent);
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                await _next(httpContext);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            //request.EnableRewind();
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = System.Text.Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void SafeLog(DateTime requestTime,
                            long responseMillis,
                            int statusCode,
                            string method,
                            string path,
                            string queryString,
                            string requestBody,
                            string responseBody)
        {
            if (path.ToLower().StartsWith("/api/users"))
            {
                requestBody = "(Request logging disabled for /api/users)";
                responseBody = "(Response logging disabled for /api/users)";
            }

            if (requestBody.Length > 5000)
            {
                requestBody = $"(Truncated to 5000 chars) {requestBody.Substring(0, 5000)}";
            }

            if (responseBody.Length > 5000)
            {
                responseBody = $"(Truncated to 5000 chars) {responseBody.Substring(0, 5000)}";
            }

            if (queryString.Length > 5000)
            {
                queryString = $"(Truncated to 5000 chars) {queryString.Substring(0, 5000)}";
            }
            if (!path.ToLower().StartsWith("/api/status"))
            {
                try
                {
                    _ = Task.Factory.StartNew(() =>
                      {
                          _ = _apiLogService.Log(new ApiLogItem
                          {
                              Id = Guid.NewGuid().ToString("N"),
                              RequestTime = requestTime,
                              ResponseMillis = responseMillis,
                              StatusCode = statusCode,
                              Method = method,
                              Path = path,
                              QueryString = queryString,
                              RequestBody = requestBody,
                              ResponseBody = responseBody,
                              RequestToken = Guid.NewGuid().ToString()
                          });
                      });
                }
                catch (Exception ex)
                {
                    string esMsg = ex.Message;
                }


            }

        }
    }
}
