namespace EduSchedule.Api.Extensions
{
    public static class ConfigureCors
    {
        public static void AddCorsPolicy(this IServiceCollection services)
        {
            string[] allowedOrigins = ["http://localhost:5173", "http://127.0.0.1:5173"];
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
    } 
}