using API.Exceptions;
using API.Logger;
using API.Middlewares;
using Infrastructure.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CatalogConnection")));

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentityPasswordOptions()
                    .AddLimitUploadFilesMax()
                    .RegisterUnitOfWork()
                    .RegisterServices()
                    .RegisterHelpers()
                    .RegisterConfigurators()
                    .AddCors(options =>
                                {
                                    options.AddPolicy("EnableCORS", builder =>
                                    {
                                        builder.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                    });
                                })
                    .AddAuthenticationJwt()
                    .ConfigureSwagger()
                    .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionInterceptor>();

            app.UseSwagger()
               .UseSwaggerUI(config =>
               {
                   config.SwaggerEndpoint("/swagger/v1/swagger.json", "Api");
                   config.RoutePrefix = string.Empty;
               })
               .UseRouting()
               .UseMiddleware<AuthTokenInterceptor>();

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseCors("EnableCORS")
                .UseAuthentication()
                .UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
