using Api.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Data.Registers
{
    public static class ContextRegister
    {
        public static void RegisterContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApiDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
