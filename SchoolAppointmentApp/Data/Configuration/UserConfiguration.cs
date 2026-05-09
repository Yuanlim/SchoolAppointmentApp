using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);

        // User must have password and email
        builder.Property(u => u.PasswordHash)
                .IsRequired();
        builder.Property(u => u.Email)
                .IsRequired();

        // User phone and email is uniquely different
        builder.HasIndex(u => u.PhoneNumber)
                .IsUnique();
        builder.HasIndex(u => u.Email)
                .IsUnique();
    }
}