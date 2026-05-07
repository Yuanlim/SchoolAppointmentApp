using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(a => a.AdminId);

        builder.Property(a => a.PasswordHash).IsRequired();
        builder.Property(a => a.AdminLoginId).IsRequired();
        builder.HasIndex(a => a.AdminLoginId).IsUnique();

        builder.HasData(
            new
            {
                AdminId = 1,
                AdminLoginId = "iwueowsakd62981sksai",
                PasswordHash = "AQAAAAIAAYagAAAAEPkLFJ63cyGZh4YEMMBflj7olrKjkCRfswg70N4NWZyONPxcarnHnhuX2zozI1OGAg=="
            },
            new
            {
                AdminId = 2,
                AdminLoginId = "84u232fhfehw889d0ufd",
                PasswordHash = "AQAAAAIAAYagAAAAEF5EzKnMIKp0aWrnmYAxClS2aiFfz0dDljh38TEU1KdwOcJnzpjiSK6Hczvs53pM1Q=="
            }
        );
    }
}