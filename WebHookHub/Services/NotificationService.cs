﻿using Hangfire;
using Hangfire.States;
using Hangfire.Tags.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WebHookHub.Filters;
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
        /// <summary>
        /// Delete Job
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        Task<bool> DeleteJob(string JobId);
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
        /// Post Data
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        public async Task<string> PostData(Models.PostDataContent postDataContent)
        {
            try
            {
                int TimeOutInMsSecs = _config.GetValue<int>("DefaultTimeOutInMiliSeconds");
                int JobDelay = _config.GetValue<int>("HangFireConfig:JobDelay");
                var idData = Guid.NewGuid().ToString();
                var DataToPostDB = new Models.DB.DataToPost()
                {
                    ID = idData,
                    Content = postDataContent.PostData,
                    RequestDate = DateTime.Now,
                    EventCode = postDataContent.EventCode,
                    ClientCode = postDataContent.ClientCode,
                    ContentBinary = Models.Utils.GeneralUtils.ToByteArray<string>(postDataContent.PostData)
                };

                List<string> listJobIds = new List<string>();
                var eventClient = _context.ClientEvents
                    .Include(x => x.ClientEventWebhooks)
                    .Include(x => x.Event)
                    .FirstOrDefault(x => x.Event.Code == postDataContent.EventCode && x.Client.Code == postDataContent.ClientCode && x.Enable);
                if (eventClient != null)
                {
                    //Get IDS
                    var ID = "";
                    var IDExtra = "";
                    if (!string.IsNullOrEmpty(eventClient.Event.RegexID))
                        ID = SearchContentUtil.SearchContent(postDataContent.PostData, eventClient.Event.RegexID);
                    if (!string.IsNullOrEmpty(eventClient.Event.RegexIDExtra))
                        IDExtra = SearchContentUtil.SearchContent(postDataContent.PostData, eventClient.Event.RegexIDExtra);
                    DataToPostDB.ContentID = ID;
                    DataToPostDB.ContentExtraID = IDExtra;
                    _context.DataToPosts.Add(DataToPostDB);
                    await _context.SaveChangesAsync();
                    try
                    {
                        if (eventClient.ClientEventWebhooks != null && eventClient.ClientEventWebhooks.Any(x => x.Enable))
                        {
                            foreach (var item in eventClient.ClientEventWebhooks.Where(x => x.Enable).ToList())
                            {

                                if (postDataContent.DelayMode == Models.DelayMode.Instant)
                                {

                                    listJobIds.Add(EnqueueRequest(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, postDataContent.PostData, postDataContent.ContentType, TimeOutInMsSecs, ID, IDExtra, item.HeaderAuthorizationValue, item.ExpectedContentResult, JobDelay, postDataContent.ParentJobID));
                                }
                                else
                                {
                                    var jobId = new BackgroundJobClient().Schedule<NotificationService>(x => x.EnqueueRequest(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, postDataContent.PostData, postDataContent.ContentType, TimeOutInMsSecs, ID, IDExtra, item.HeaderAuthorizationValue, item.ExpectedContentResult, JobDelay, postDataContent.ParentJobID), TimeSpan.FromSeconds(postDataContent.DelayValue.Value));
                                    listJobIds.Add(jobId);
                                }

                                _logger.LogInformation(postDataContent.ToString());
                            }
                            if (listJobIds.Any() && !string.IsNullOrEmpty(postDataContent.CustomJobID))
                            {
                                await _context.CustomJobIDs.AddAsync(new Models.DB.CustomJobID { ExternalJobID = postDataContent.CustomJobID, InternalJobID = listJobIds.FirstOrDefault(), CreationDateTime = DateTime.Now });
                                await _context.SaveChangesAsync();
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
                        _logger.LogError(exevent.Message);
                        throw new Exception(eventClient.ContentReponseError);
                    }


                }
                else
                {
                    _context.DataToPosts.Add(DataToPostDB);
                    await _context.SaveChangesAsync();
                    _logger.LogError("Not Webhooks Configured");
                    return "[error][configuration not found]";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return "[error][" + ex.Message + " : " + ex.StackTrace + "]";
            }

        }


        /// <summary>
        /// DeleteJob
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteJob(string JobId)
        {
            try
            {
                await Task.FromResult(true);
                var jobId = new BackgroundJobClient().Delete(JobId);
                return jobId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return false;
            }

        }

        /// <summary>
        /// Send Data to WebHooks
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <param name="EventCode"></param>
        /// <param name="urlToPost"></param>
        /// <param name="dataToPost"></param>
        /// <param name="contentType"></param>
        /// <param name="TimeOutInMsSecs"></param>
        /// <param name="ID"></param>
        /// <param name="IDExtra"></param>
        /// <param name="HeaderAuthorizationValue"></param>
        /// <param name="expectedResult"></param>
        /// <param name="dealyJob"></param>
        /// <param name="parentJobId"></param>
        /// <returns></returns>
        [DisplayName("SendData: [{1}:{0}][{6} | {7}]")]
        [Tag("SendData", "{0}", "{1}")]
        public string SendData(string ClientCode, string EventCode, string urlToPost, string dataToPost, string contentType, int TimeOutInMsSecs, string ID, string IDExtra, string HeaderAuthorizationValue, string expectedResult = "", int dealyJob = 0, string parentJobId = "")
        {
            if (dealyJob != 0)
            {
                System.Threading.Thread.Sleep(dealyJob);
            }
            using (CustomWebClient wc = new CustomWebClient(TimeOutInMsSecs))
            {
                //Header Autorization Value
                if (!string.IsNullOrEmpty(HeaderAuthorizationValue))
                {
                    wc.Headers[HttpRequestHeader.Authorization] = HeaderAuthorizationValue;
                }

                wc.Headers[HttpRequestHeader.ContentType] = contentType;
                wc.Headers.Add("WebhookHub_ClientCode", ClientCode);
                wc.Headers.Add("WebhookHub_EventCode", EventCode);
                wc.Headers.Add("WebhookHub_ID", ID);
                wc.Headers.Add("WebhookHub_IDExtra", IDExtra);
                string reponse = wc.UploadString(new Uri(urlToPost), dataToPost);

                if (!string.IsNullOrEmpty(expectedResult) && !reponse.Contains(expectedResult))
                {
                    throw new Exception("[Unexpected content result]" + reponse);
                }
                return reponse;
            }
        }

        /// <summary>
        /// Enqueue Request
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <param name="EventCode"></param>
        /// <param name="urlToPost"></param>
        /// <param name="dataToPost"></param>
        /// <param name="contentType"></param>
        /// <param name="TimeOutInMsSecs"></param>
        /// <param name="ID"></param>
        /// <param name="IDExtra"></param>
        /// <param name="HeaderAuthorizationValue"></param>
        /// <param name="expectedResult"></param>
        /// <param name="dealyJob"></param>
        /// <param name="parentJobId"></param>
        /// <returns></returns>
        [DisplayName("EnqueueRequest: [{1}:{0}][{6} | {8}]")]
        [Tag("EnqueueRequest", "{0}", "{1}")]
        public string EnqueueRequest(string ClientCode, string EventCode, string urlToPost, string dataToPost, string contentType, int TimeOutInMsSecs, string ID, string IDExtra, string HeaderAuthorizationValue, string expectedResult = "", int dealyJob = 0, string parentJobId = "")
        {
            EnqueuedState queue = new EnqueuedState(ClientCode.ToLower());
            if (!string.IsNullOrEmpty(parentJobId))
            {
                var customJob = _context.CustomJobIDs.Where(x => x.ExternalJobID == parentJobId).OrderByDescending(x => x.CreationDateTime).FirstOrDefault();
                if (customJob != null)
                {
                    var jobId = new BackgroundJobClient().ContinueJobWith<NotificationService>(customJob.InternalJobID, x => x.SendData(ClientCode, EventCode, urlToPost, dataToPost, contentType, TimeOutInMsSecs, ID, IDExtra, HeaderAuthorizationValue, expectedResult, dealyJob, parentJobId), queue);
                    return jobId;
                }
                else
                {
                    var jobId = new BackgroundJobClient().Create<NotificationService>(x => x.SendData(ClientCode, EventCode, urlToPost, dataToPost, contentType, TimeOutInMsSecs, ID, IDExtra, HeaderAuthorizationValue, expectedResult, dealyJob, parentJobId), queue);
                    return jobId;
                }
            }
            else
            {
                var jobId = new BackgroundJobClient().Create<NotificationService>(x => x.SendData(ClientCode, EventCode, urlToPost, dataToPost, contentType, TimeOutInMsSecs, ID, IDExtra, HeaderAuthorizationValue, expectedResult, dealyJob, parentJobId), queue);
                return jobId;
            }
        }

    }
}
