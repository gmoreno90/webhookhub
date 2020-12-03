using Hangfire;
using Hangfire.States;
using Hangfire.Tags.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebHookHub.Models.Utils;

namespace WebHookHub.Services
{
    /// <summary>
    /// Inotification Service
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Post Data
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        Task<string> PostData(Models.PostDataContent postDataContent);
    }
    /// <summary>
    /// NotificationService
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly Models.DB.WebHookHubContext _context;
        private IConfiguration _config { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="config"></param>
        public NotificationService(ILogger<NotificationService> logger, Models.DB.WebHookHubContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        public async Task<string> PostData(Models.PostDataContent postDataContent)
        {
            try
            {
                int TimeOutInMsSecs = _config.GetValue<int>("DefaultTimeOutInMiliSeconds");
                List<string> listJobIds = new List<string>();
                var eventClient = _context.ClientEvents.Include(x=>x.ClientEventWebhooks).FirstOrDefault(x => x.Event.Code == postDataContent.EventCode && x.Client.Code == postDataContent.ClientCode && x.Enable);
                if (eventClient != null)
                {
                    try
                    {
                        if (eventClient.ClientEventWebhooks != null && eventClient.ClientEventWebhooks.Count() > 0)
                        {
                            foreach (var item in eventClient.ClientEventWebhooks)
                            {
                                EnqueuedState queue = new EnqueuedState(postDataContent.ClientCode.ToLower());
                                //var jobId = BackgroundJob.Enqueue(() => SendData(item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType));
                                var jobId = new BackgroundJobClient().Create<NotificationService>(x => x.SendData(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType, TimeOutInMsSecs, item.ExpectedContentResult), queue);
                                listJobIds.Add(jobId);
                                _logger.LogInformation(postDataContent.ToString());
                            }
                            return eventClient.ContentReponseOk;
                        }
                        else
                        {
                            _logger.LogInformation("Not Webhooks Configured");
                            return eventClient.ContentReponseError;
                        }
                    }
                    catch (Exception exevent)
                    {
                        throw new Exception(eventClient.ContentReponseError);
                    }
                    

                }
                else {
                    _logger.LogError("Not Webhooks Configured");
                    return "[error][configuration not found]";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return "[error][" + ex.Message + " : " + ex.StackTrace +"]";
            }

        }
        /// <summary>
        /// Send Data to WebHooks
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <param name="EventCode"></param>
        /// <param name="urlToPost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="dataToPost"></param>
        /// <param name="contentType"></param>
        /// <param name="TimeOutInMsSecs"></param>
        [DisplayName("SendData: [{1}:{0}]")]
        [Tag("SendData", "{0}", "{1}")]
        public void SendData(string ClientCode, string EventCode, string urlToPost, string userName, string passWord, string dataToPost, string contentType, int TimeOutInMsSecs, string expectedResult = "")
        {
            using (CustomWebClient wc = new CustomWebClient(TimeOutInMsSecs))
            {
                if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(passWord))
                {
                    NetworkCredential myCreds = new NetworkCredential(userName, passWord);
                    wc.Credentials = myCreds;
                }
                wc.Headers[HttpRequestHeader.ContentType] = contentType;
                string HtmlResult = wc.UploadString(new Uri(urlToPost), dataToPost);

                if (string.IsNullOrEmpty(expectedResult)) {
                    if (HtmlResult != expectedResult) {
                        throw new Exception("[Unexpected content result]");
                    }
                }

            }
        }
    }
}
