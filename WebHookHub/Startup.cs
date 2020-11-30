using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using System.IO;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Http;

namespace WebHookHub
{
    public class Startup
    {
        const string VERSION = "1.0.0";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Models.DB.WebHookHubContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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
                    }));
            //Retry Intervals
            var TimeIntervals = Configuration.GetSection("HangFireConfig:HangFireRetryIntervalInSeconds").Get<int[]>();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = TimeIntervals.Length, DelaysInSeconds = TimeIntervals, OnAttemptsExceeded = AttemptsExceededAction.Fail });
            //Presrve Queue
            GlobalJobFilters.Filters.Add(new Filters.PreserveOriginalQueueAttribute());

            services.AddTransient<Services.ApiLogService>();
            services.AddSingleton<Services.INotificationService, Services.NotificationService>();

            // Add the processing server as IHostedService
            //services.AddHangfireServer();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.Use((context, next) =>
            //{
            //    context.Request.EnableBuffering();
            //    return next();
            //});
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
            app.UseHangfireDashboard(Configuration.GetValue<string>("HangFireConfig:DashboardPath"), dashboardOptions);
            var options = new BackgroundJobServerOptions
            {
                Queues = GetQueuesHangFire(app).ToArray()
            };

            app.UseHangfireServer(options);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<Models.DB.WebHookHubContext>();

            context.Database.Migrate();
        }
        private List<string> GetQueuesHangFire(IApplicationBuilder app)
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
