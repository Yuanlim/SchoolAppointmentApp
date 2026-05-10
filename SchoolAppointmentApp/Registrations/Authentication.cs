using Microsoft.AspNetCore.Identity;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.Registrations;

/// <summary>
/// Meaning of life: user trying to identify himself
/// </summary>
/// <param name="builder"></param>
internal static class Authenticate
{
    /// <summary>
    /// Set Auth type => cookie
    /// Configure cookie => when cookie checked fail / no permission => 403
    ///                 => no cookie but want to use an restricted api => redirect to login => 401
    /// </summary>
    /// <param name="builder"></param>
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Cookie")
                .AddCookie("Cookie", c =>
                {
                    // set behavior on when user not logged in but access some endpoint
                    c.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        return Task.CompletedTask;
                    };

                    c.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };

                    c.LoginPath = "/login";
                    c.LogoutPath = "/logout";
                    c.ExpireTimeSpan = TimeSpan.FromHours(8);
                });
    }
}