using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Library.API.Configuration;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Services;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.API;

public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Configure JWT Authentication
        var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT Secret Key is not configured")))
                };
            });

        // Configure Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
            
            // Configure Swagger to use JWT Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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
                    Array.Empty<string>()
                }
            });
        });

        // Configure Database
        // If running in test environment, skip DB registration here so tests can replace it with InMemory provider
        if (!_env.IsEnvironment("Testing"))
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
        }

        // Configure Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    // Media repository and services
    services.AddScoped<Library.Core.Interfaces.IMediaItemRepository, Library.Infrastructure.Repositories.MediaItemRepository>();
    services.AddScoped<Library.Infrastructure.Services.MediaItemService>();
    // Application services
    services.AddScoped<Library.Application.Services.SearchService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
