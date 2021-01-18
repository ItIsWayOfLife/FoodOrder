using API.Configurators;
using API.Helpers;
using API.Interfaces;
using Core.Helper;
using Core.Identity;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace API.Exceptions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddIdentityPasswordOptions(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(
                opts =>
                {
                    opts.Password.RequiredLength = 6;   //minimum length
                    opts.Password.RequireNonAlphanumeric = false;   // whether non-alphanumeric characters are required
                    opts.Password.RequireLowercase = false; // whether lowercase characters are required
                    opts.Password.RequireUppercase = false; // whether uppercase characters are required
                    opts.Password.RequireDigit = false; // are numbers required
                })
                .AddEntityFrameworkStores<IdentityContext>();

            return services;
        }

        // limit for upload files
        public static IServiceCollection AddLimitUploadFilesMax(this IServiceCollection services)
        {
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            return services;
        }

        public static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, EFUnitOfWork>();

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<ILoggerService, LoggerService>();
            services.AddTransient<IProviderService, ProviderService>();
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<IDishService, DishService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IReportService, ReportService>();

            return services;
        }

        public static IServiceCollection RegisterHelpers(this IServiceCollection services)
        {
            services.AddTransient<IUserHelper, UserHelper>();

            services.AddTransient<IProviderHelper, ProviderHelper>();
            services.AddTransient<ICatalogHelper, CatalogHelper>();
            services.AddTransient<IDishHelper, DishHelper>();
            services.AddTransient<ICartHelper, CartHelper>();
            services.AddTransient<IOrderHelper, OrderHelper>();
            services.AddTransient<IMenuHelper, MenuHelper>();

            return services;
        }

        public static IServiceCollection RegisterConfigurators(this IServiceCollection services)
        {
            services.AddTransient<IJwtConfigurator, JwtConfigurator>();

            return services;
        }

        public static IServiceCollection AddAuthenticationJwt(this IServiceCollection services)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,

                  ValidIssuer = "https://localhost:44342",
                  ValidAudience = "https://localhost:44342",
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
              };
          });

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(config =>
            {
                var authenticationTypeId = "Authentication";
                config.AddSecurityDefinition(authenticationTypeId, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = authenticationTypeId
                            },
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                var apiDocs = Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml");
                config.IncludeXmlComments(apiDocs);

            });
        }
    }
}
