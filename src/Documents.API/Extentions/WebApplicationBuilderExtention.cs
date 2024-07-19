using Documents.API.Services;
using Documents.API.Storage;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Documents.API.Extentions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBlobService, BlobService>();

        builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "https://localhost:5005";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "documents.api");
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blob"]!, preferMsi: true);
            clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnectionString:queue"]!, preferMsi: true);
        });

    }
}
