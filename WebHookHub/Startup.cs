using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Heartbeat;
using Hangfire.Heartbeat.Server;
using Hangfire.SqlServer;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace WebHookHub
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// VERSION
        /// </summary>
        const string VERSION = "1.0.0";
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<Models.DB.WebHookHubContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfire(config =>
            {
                //SqlServer Sample
                config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // To enable Sliding invisibility fetching
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // To enable command pipelining
                    QueuePollInterval = TimeSpan.FromTicks(1) // To reduce processing delays to minimum
                });
                var options = new TagsOptions
                {
                    TagsListStyle = TagsListStyle.Dropdown
                };
                config.UseTagsWithSql(options);
                //end SqlServer Sample

                config.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1));
            });
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    })
                    .UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1))
                    .UseTagsWithSql()
                    .UseMaxArgumentSizeToRender(Configuration.GetValue<int>("HangFireConfig:MaxArgumentToRenderSize"))
                    );
            //Retry Intervals
            var TimeIntervals = Configuration.GetSection("HangFireConfig:HangFireRetryIntervalInSeconds").Get<int[]>();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = TimeIntervals.Length, DelaysInSeconds = TimeIntervals, OnAttemptsExceeded = AttemptsExceededAction.Fail });
            //Presrve Queue
            GlobalJobFilters.Filters.Add(new Filters.CustomHangfireFilterAttribute());

            services.AddTransient<Services.ApiLogService>();
            services.AddTransient<Services.INotificationService, Services.NotificationService>();

            // Add the processing server as IHostedService
            services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.WriteIndented = true;
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddXmlSerializerFormatters();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(VERSION, new OpenApiInfo
                {
                    Version = VERSION,
                    Title = "WebHook HUB API",
                    Description = "",
                    Contact = new OpenApiContact
                    {
                        Name = "Gonzalo Moreno",
                        Email = "gonzalo.moreno@solutions2az.net",
                        Url = new Uri("https://github.com/gmoreno90"),
                    }
                });

                var dir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory));
                foreach (var fi in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(fi.FullName);
                }
            });
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            UpdateDatabase(app);
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + VERSION + "/swagger.json", "WebHook HUB API " + VERSION);
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<Middleware.ApiLoggingMiddleware>();

            var dashboardOptions = new DashboardOptions
            {
                DashboardTitle = "WebHook Hub - HangFire",
                Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = Configuration.GetValue<string>("HangFireConfig:DashboardUserName"),
                            PasswordClear =  Configuration.GetValue<string>("HangFireConfig:DashboardPassword")
                        }
                    }

                }) }
            };
            NavigationMenu.Items.Add(page => new MenuItem("API Documentation", "/index.html"));
            app.UseHangfireDashboard(Configuration.GetValue<string>("HangFireConfig:DashboardPath"), dashboardOptions);

            var options = new BackgroundJobServerOptions
            {
                Queues = GetQueuesHangFire(app).ToArray()
            };
            app.UseHangfireServer(options, additionalProcesses: new[] { new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        /// <summary>
        /// UpdateDatabase
        /// </summary>
        /// <param name="app"></param>
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<Models.DB.WebHookHubContext>();

            context.Database.Migrate();
        }
        /// <summary>
        /// GetQueuesHangFire
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static List<string> GetQueuesHangFire(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<Models.DB.WebHookHubContext>();
            var res = context.Clients.Select(x => x.Code).ToList();
            res.Add("default");
            return res;
        }
    }
}
