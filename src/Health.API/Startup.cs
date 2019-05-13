﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Health;
using Utils;
using Services.OAuth;
using Repositories.OAuth;
using System.Net.Http;
using Hangfire;
using Hangfire.MemoryStorage;
using HealthAPI.Hangfire;
using Importer.Fitbit;
using Importer.Fitbit.Importer;
using Importer.GoogleSheets;
using Importer.Withings;
using Services.Health;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using Calendar = Utils.Calendar;

namespace HealthAPI
{
    public class Startup
    {
        private IConfig _config;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = new Config();
        }

        public IConfiguration Configuration { get; }

        public virtual void SetUpDataBase(IServiceCollection services)
        {
            

            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<HealthContext>(dboptions =>
            {
                dboptions.UseSqlServer(
                    _config.HealthDbConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);

                        sqlOptions.MigrationsAssembly("Repositories");
                    }
                );
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                //options.AddDefaultPolicy(
                    //builder =>
                    //{
                    //    builder.AllowAnyOrigin()
                    //        .AllowAnyMethod()
                    //        .AllowAnyHeader()
                    //        .AllowCredentials();
                    //});

                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
//                        .AllowCredentials());
            });

            SetUpDataBase(services);

            services.AddHangfire(x => x.UseMemoryStorage());

            services.AddMvc(o => { o.Filters.Add<GlobalExceptionFilter>(); });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddScoped<IHealthRepository, HealthRepository>();

            services.AddSingleton<HttpClient, HttpClient>();

            services.AddTransient<IHealthService, HealthService>();

            services.AddTransient<IConfig, Config>();

            services.AddTransient<ILogger, LogzIoLogger>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IFitbitAuthenticator, FitbitAuthenticator>();
            services.AddTransient<IFitbitClientQueryAdapter, FitbitClientQueryAdapter>();
            services.AddTransient<IFitbitService, FitbitService>();
            services.AddTransient<IFitbitImporter, FitbitImporter>();
            services.AddTransient<ICalendar, Calendar>();
            services.AddTransient<IWithingsClient, WithingsClient>();
            services.AddTransient<IWithingsAuthenticator, WithingsAuthenticator>();
            services.AddTransient<IFitbitClient, FitbitClient>();
            services.AddTransient<ISheetsService, SheetsService>();

            services.AddTransient<IHangfireWork, HangfireWork>();
            services.AddTransient<IWithingsService, WithingsService>();
            services.AddTransient<IFitbitMapper, FitbitMapper>();
            services.AddTransient<IRowMapper, RowMapper>();
            services.AddTransient<IMapFunctions, MapFunctions>();
            services.AddTransient<ISheetsClient, SheetsClient>();


            services.AddTransient<IWithingsMapper, WithingsMapper>();
            services.AddTransient<IWithingsClientQueryAdapter, WithingsClientQueryAdapter>();

            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();


            //// ********************
            //// Setup CORS
            //// ********************
            //var corsBuilder = new CorsPolicyBuilder();
            //corsBuilder.AllowAnyHeader();
            //corsBuilder.AllowAnyMethod();
            //corsBuilder.AllowAnyOrigin(); // For anyone access.
            ////corsBuilder.WithOrigins("http://localhost:56573"); // for a specific url. Don't add a forward slash on the end!
            ////corsBuilder.AllowCredentials();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //app.UseCors();
            app.UseCors("CorsPolicy");
            //app.UseCors(builder => builder.WithOrigins("http://www.musgrosoft.co.uk"));

            app.UseMvc();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<HealthContext>();
                //context.Database.Migrate();
                // context.EnsureSeedData();
                context.Database.EnsureCreated();
                
            }

        }
    }
}
