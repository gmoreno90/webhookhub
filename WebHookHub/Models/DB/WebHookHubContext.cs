using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// WebHookHubContext
    /// </summary>
    public class WebHookHubContext : DbContext
    {
        private readonly IConfiguration _config;
        /// <summary>
        /// WebHookHubContext
        /// </summary>
        /// <param name="options"></param>
        /// <param name="config"></param>
        public WebHookHubContext(DbContextOptions<WebHookHubContext> options, IConfiguration config) : base(options)
        {
            _config = config;
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
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
