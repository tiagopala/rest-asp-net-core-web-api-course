using Api.Application.ApplicationServices;
using Api.Business.Interfaces;
using Api.Business.Notifications;
using Api.Business.Services;
using Api.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Application.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<INotifier, Notifier>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Repositories
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            #endregion

            #region Services
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IUserService, UserService>();
            #endregion
        }
    }
}
