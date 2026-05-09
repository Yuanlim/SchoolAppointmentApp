using System.ComponentModel.DataAnnotations;

namespace SchoolAppointmentApp.Entities;

// Not link with users cause you cant block, friend or report Admin
public class Admin
{
    public int AdminId { get; set; }
    public required string AdminLoginId { get; set; }
    public required string PasswordHash { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Contact { get; set; }
}