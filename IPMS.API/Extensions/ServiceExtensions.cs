using Core.Entities;
using Core.Interfaces;
using Data;
using Data.Services;
using IPMS.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IPMS.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddDbContext<InsuranceContext>(options =>
                options.UseSqlServer(config.GetConnectionString("IPMS")));
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = config.GetConnectionString("Redis");

                opt.InstanceName = "IPMS_";
            });
            services.AddAutoMapper(typeof(InsuranceProfile));
            services.AddCors();
            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddDbContext<InsuranceContext>(options =>
                options.UseSqlServer(config.GetConnectionString("IPMS")));

            services.AddIdentity<SystemUser, IdentityRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<InsuranceContext>()
              .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(1);
            });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"]!)),
                        ValidIssuer = config["Token:Issuer"],
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };

                });

            return services;
        }
    }
}
