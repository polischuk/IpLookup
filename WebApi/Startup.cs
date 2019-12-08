using System.IO.Compression;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Filters;

namespace WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterDIServices(services);
            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddMvcCore(options =>
            {
                options.Filters.Add(new ApiControllerAttribute());
                options.Filters.Add(new ModelStateValidationActionFilterAttribute());
                options.EnableEndpointRouting = false;
            })
                .AddFormatterMappings()
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        builder =>
                        {
                            builder
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                }).AddApiExplorer()
                .AddNewtonsoftJson()
                .AddDataAnnotations()
                .AddAuthorization().SetCompatibilityVersion(CompatibilityVersion.Latest);

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            builder.Build();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc();
        }

        protected void RegisterDIServices(IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            services.AddSingleton<IMongoDbConfig, MongoDbConfig>();
            services.AddSingleton<IIpService, IpService>();
        }
    }
}
