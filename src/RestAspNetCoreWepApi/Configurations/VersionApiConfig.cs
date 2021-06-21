using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Application.Configurations
{
    public static class VersionApiConfig
    {
        public static void ResolveVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; // Assumindo a versão default quando não especificado.
                options.DefaultApiVersion = new ApiVersion(1, 0);   // Definindo a versão default da api.
                options.ReportApiVersions = true; // Reportar durante a chamada se a versão utilizada é a atual ou se ela está depreciada.
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // Definindo o formado de versões da Api. Exemplo: v1.0.0.
                options.SubstituteApiVersionInUrl = true; // Substituir a versão na url da api caso necessário.
            });
        }
    }
}
