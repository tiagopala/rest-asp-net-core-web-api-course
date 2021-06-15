using Api.Business.Interfaces;
using Api.Business.Notifications;
using Api.Business.Services;
using Api.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Application.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<INotifier, Notifier>();

            #region Repositories
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            #endregion

            #region Services
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            #endregion
        }
    }
}
