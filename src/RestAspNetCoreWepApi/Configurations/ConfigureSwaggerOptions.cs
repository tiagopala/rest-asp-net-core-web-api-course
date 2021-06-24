using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Api.Application.Configurations
{
    public static class SwaggerOptions
    {
        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
            });
        }

        public static void UseSwaggerConfig(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }
    }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateOpenApiInfoForeachVersion(description));
            }
        }

        private OpenApiInfo CreateOpenApiInfoForeachVersion(ApiVersionDescription description)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = "Api - RestAspNetCoreWebApi",
                Version = description.ApiVersion.ToString(),
                Description = "Api criada no curso REST com ASP.NET Core WebApi - desenvolvedor.io",
                Contact = new OpenApiContact { Name = "Tiago Pala", Email = "tiago_pala@outlook.com"},
                TermsOfService = new Uri("https://opensource.org/licenses/MIT"),
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
                openApiInfo.Description += "Essa versão está obsoleta!";

            return openApiInfo;
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters is null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description is null)
                    parameter.Description = description.ModelMetadata?.Description;

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
