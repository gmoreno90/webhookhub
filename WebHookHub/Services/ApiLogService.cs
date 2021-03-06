﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebHookHub.Models.DB;

namespace WebHookHub.Services
{
    /// <summary>
    /// Interface of Api Log
    /// </summary>
    public interface IApiLogService
    {
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="apiLogItem"></param>
        /// <returns></returns>
        Task Log(ApiLogItem apiLogItem);
        /// <summary>
        /// Remove Old Elements of logs
        /// </summary>
        /// <returns></returns>
        Task<bool> AutoMaintenance();

    }

    /// <summary>
    /// Api log
    /// </summary>
    public class ApiLogService : IApiLogService
    {
        private readonly ILogger<ApiLogService> _logger;
        private IConfiguration _config { get; }
        private readonly Models.DB.WebHookHubContext _db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ApiLogService(ILogger<ApiLogService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            var contextOptionBuilder = new DbContextOptionsBuilder<Models.DB.WebHookHubContext>();
            contextOptionBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            _db = new Models.DB.WebHookHubContext(contextOptionBuilder.Options);
        }
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="apiLogItem"></param>
        /// <returns></returns>
        public async Task Log(ApiLogItem apiLogItem)
        {
            try
            {
                _logger.LogInformation(apiLogItem.RequestBody);
                await _db.ApiLogItems.AddAsync(apiLogItem);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
            }
        }


        /// <summary>
        /// Remove Old Elements of logs
        /// </summary>
        public async Task<bool> AutoMaintenance()
        {
            try
            {
                _logger.LogInformation("Purging Logs");
                var DaysToPurge = _config.GetValue<int>("LogsDaysPurge");
                DateTime purgeDate = DateTime.Now.AddDays(-DaysToPurge);
                var entriesTodelete = await _db.ApiLogItems.Where(x => x.RequestTime <= purgeDate).ToListAsync();
                foreach (var logItem in entriesTodelete)
                {
                    _db.ApiLogItems.Remove(logItem);
                }
                _db.SaveChanges();
                _logger.LogInformation("Purging Logs Complete");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error Purging Logs");
                _logger.LogError(ex.Message);
                return false;
            }

        }
    }
}
