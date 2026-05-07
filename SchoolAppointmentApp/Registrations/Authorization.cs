using System.Security.Claims;

namespace SchoolAppointmentApp.Registrations;

/// <summary>
/// Meaning of life: Check if the authenticated person has which permission
/// </summary>
/// <param name="builder"></param>
internal static class Authorization
{
    /// <summary>
    /// Adding own policies so specific role of people can visit
    /// </summary>
    /// <param name="builder"></param>
    public static void AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(
            o =>
            {
                o.AddPolicy(
                    "TeacherAllowed", policy => policy.RequireRole("teacher")
                                                        .RequireClaim("TeacherId")
                );
                o.AddPolicy(
                    "StudentAllowed", policy => policy.RequireRole("student")
                                                        .RequireClaim("StudentId")
                );
                o.AddPolicy(
                    "AdminAllowed", policy => policy.RequireRole("admin")
                                                        .RequireClaim(ClaimTypes.NameIdentifier)
                );
                o.AddPolicy(
                    "PrincipalAllowed", policy => policy.RequireRole("schoolPrincipal")
                                                        .RequireClaim(ClaimTypes.NameIdentifier)
                );
                o.AddPolicy(
                    "TeacherOrStudentAllowed", policy => policy.RequireRole("student", "teacher")
                );
                o.AddPolicy(
                    "TeacherOrPrincipalAllowed", policy => policy.RequireRole("schoolPrincipal", "teacher")
                );
                o.AddPolicy(
                    "AllRoleAllowed", policy => policy.RequireRole("student", "teacher", "admin", "schoolPrincipal")
                );
            }
        );
    }
}