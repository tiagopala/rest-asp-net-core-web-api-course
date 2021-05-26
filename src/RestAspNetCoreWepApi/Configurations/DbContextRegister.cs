using Api.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Application.Configurations
{
    public static class DbContextRegister
    {
        public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApiDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
