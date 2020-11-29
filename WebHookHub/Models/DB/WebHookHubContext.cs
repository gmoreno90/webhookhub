using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    public class WebHookHubContext : DbContext
    {
        private readonly IConfiguration _config;
        public WebHookHubContext(DbContextOptions<WebHookHubContext> options, IConfiguration config) : base(options) {
            _config = config;
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ClientEvent> ClientEvents { get; set; }
        public DbSet<ApiLogItem> ApiLogItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
