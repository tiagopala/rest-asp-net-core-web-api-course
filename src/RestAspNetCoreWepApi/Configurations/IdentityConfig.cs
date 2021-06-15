using Api.Application.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Application.Configurations
{
    public static class IdentityConfig
    {
        public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Fix
            //services.AddIdentityCore<IdentityUser>()
            //        .AddRoles<IdentityRole>()
            //        .AddEntityFrameworkStores<AuthDbContext>()
            //        .AddDefaultTokenProviders();
        }
    }
}
