using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;

namespace Polly.Api.Configurations.Documentation
{
    public static class ApiDocumentationConfiguration
    {
        public static void AddCustomApiDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();
                options.SwaggerDoc("Api", new OpenApiInfo
                {
                    Title = "Polly Api",
                    Version = "v1",
                    Description = "Api to example implementation polly",
                    Contact = new OpenApiContact
                    {
                        Name = "Nuno Mendes",
                        Email = "nunomendes88@hotmail.com",
                        Url = new Uri("https://www.nunomendes.net/"),
                    },
                });
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                options.IncludeXmlComments(xmlCommentsFullPath);
            });
        }

        public static void UseCustomApiDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/Api/swagger.json", "Polly Api");
                options.RoutePrefix = string.Empty;
                options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
            });
        }
    }
}
