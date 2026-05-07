using System.Security.Claims;

namespace SchoolAppointmentApp.Registrations;

/// <summary>
/// Meaning of life: which domain allow to visit this backend
/// </summary>
/// <param name="builder"></param>
internal static class CORS
{
    /// <summary>
    /// Adding domain that can visit this backend
    /// </summary>
    /// <param name="builder"></param>
    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            o => o.AddPolicy("FrontendCorsPolicy",
                policy =>
                {
                    policy.WithOrigins(["https://localhost:3000/", "https://localhost:3001"]);
                    policy.AllowAnyMethod();
                    policy.WithHeaders("Content-Type");
                }
            )
        );
    }
}