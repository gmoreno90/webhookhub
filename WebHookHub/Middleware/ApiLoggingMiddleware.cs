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

                        SafeLog(new SafeLogRQ()
                        {
                            requestTime = requestTime,
                            responseMillis = stopWatch.ElapsedMilliseconds,
                            statusCode = response.StatusCode,
                            method = request.Method,
                            path = request.Path,
                            queryString = request.QueryString.ToString(),
                            requestBody = requestBodyContent,
                            responseBody = responseBodyContent
                        });
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception)
            {
                await _next(httpContext);
            }
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            Memory<byte> memory = new Memory<byte>(buffer);
            await request.Body.ReadAsync(memory);
            var bodyAsText = System.Text.Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void SafeLog(SafeLogRQ rq)
        {
            if (rq.path.ToLower().StartsWith("/api/users"))
            {
                rq.requestBody = "(Request logging disabled for /api/users)";
                rq.responseBody = "(Response logging disabled for /api/users)";
            }

            if (rq.requestBody.Length > 5000)
            {
                rq.requestBody = $"(Truncated to 5000 chars) {rq.requestBody.Substring(0, 5000)}";
            }

            if (rq.responseBody.Length > 5000)
            {
                rq.responseBody = $"(Truncated to 5000 chars) {rq.responseBody.Substring(0, 5000)}";
            }

            if (rq.queryString.Length > 5000)
            {
                rq.queryString = $"(Truncated to 5000 chars) {rq.queryString.Substring(0, 5000)}";
            }
            if (!rq.path.ToLower().StartsWith("/api/status"))
            {
                try
                {
                    _ = Task.Factory.StartNew(() =>
                      {
                          _ = _apiLogService.Log(new ApiLogItem
                          {
                              Id = Guid.NewGuid().ToString("N"),
                              RequestTime = rq.requestTime,
                              ResponseMillis = rq.responseMillis,
                              StatusCode = rq.statusCode,
                              Method = rq.method,
                              Path = rq.path,
                              QueryString = rq.queryString,
                              RequestBody = rq.requestBody,
                              ResponseBody = rq.responseBody,
                              RequestToken = Guid.NewGuid().ToString()
                          });
                      });
                }
                catch (Exception)
                {
                    //We can ignore becouse is not usefull data when crash the logs
                }


            }

        }

        public class SafeLogRQ
        {
            public DateTime requestTime { get; set; }
            public long responseMillis { get; set; }
            public int statusCode { get; set; }
            public string method { get; set; }
            public string path { get; set; }
            public string queryString { get; set; }
            public string requestBody { get; set; }
            public string responseBody { get; set; }
        }
    }
}
