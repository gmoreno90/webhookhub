using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// WebHookHubContext
    /// </summary>
    public class WebHookHubContext : DbContext
    {
        /// <summary>
        /// WebHookHubContext
        /// </summary>
        /// <param name="options"></param>
        public WebHookHubContext(DbContextOptions<WebHookHubContext> options) : base(options)
        {
        }
        /// <summary>
        /// Clients
        /// </summary>
        public DbSet<Client> Clients { get; set; }
        /// <summary>
        /// Events
        /// </summary>
        public DbSet<Event> Events { get; set; }
        /// <summary>
        /// ClientEvents
        /// </summary>
        public DbSet<ClientEvent> ClientEvents { get; set; }
        /// <summary>
        /// Client Event Webhooks
        /// </summary>
        public DbSet<ClientEventWebhooks> ClientEventWebhooks { get; set; }
        /// <summary>
        /// ApiLogItems
        /// </summary>
        public DbSet<ApiLogItem> ApiLogItems { get; set; }

        /// <summary>
        /// DataToPost
        /// </summary>
        public DbSet<DataToPost> DataToPosts { get; set; }
        /// <summary>
        /// CustomJobID
        /// </summary>
        public DbSet<CustomJobID> CustomJobIDs { get; set; }
        


    }
}
