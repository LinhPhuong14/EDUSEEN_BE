namespace Sep490_Eduseen_BE.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddCors(options => {
            options.AddPolicy("DefaultCorsPolicy", builder => {
                if (environment.IsDevelopment())
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                        Console.WriteLine("CORS: Development mode - Allowing all origins.");
                    }
                    else
                    {
                        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                        if (allowedOrigins != null && allowedOrigins.Any())
                        {
                            builder.WithOrigins(allowedOrigins)
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials();
                            Console.WriteLine($"CORS: Production mode - Allowed origins: {string.Join(", ", allowedOrigins)}");
                        }
                        else
                        {
                            throw new InvalidOperationException("CORS allowed origins are not configured for production.");
                        }
                    }
            });
        });
        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseCors("DefaultCorsPolicy");
        }
        else
        {
            app.UseCors("DefaultCorsPolicy");
        }

        return app;
    }
}