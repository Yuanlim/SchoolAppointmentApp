using System.Text.Json.Serialization;

namespace SchoolAppointmentApp.Registrations;

/// <summary>
/// Meaning of life: which domain allow to visit this backend
/// </summary>
/// <param name="builder"></param>
internal static class Converter
{
    /// <summary>
    /// Adding auto enum converter, when the incoming request is enum
    /// </summary>
    /// <param name="builder"></param>
    public static void AddConverter(this WebApplicationBuilder builder)
    {
        // Treat passed in string able to convert to enum
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}