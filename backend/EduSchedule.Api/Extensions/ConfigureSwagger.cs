using System;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace EduSchedule.Api.Extensions;

public static class ConfigureSwagger
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
            { 
                Name = "Authorization", 
                Type = SecuritySchemeType.Http, 
                Scheme = "Bearer", 
                BearerFormat = "JWT", 
                In = ParameterLocation.Header, 
                Description = "Cole apenas o seu token JWT abaixo.", 
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement 
            { 
                { 
                    new OpenApiSecurityScheme 
                    { 
                        Reference = new OpenApiReference 
                        { 
                            Type = ReferenceType.SecurityScheme, 
                            Id = "Bearer" 
                        } 
                    }, 
                    new string[] {} 
                } 
            }); 
        });
    }
}
