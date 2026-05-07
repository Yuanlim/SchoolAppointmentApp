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
                PasswordHash = "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A=="
            },
            new
            {
                AdminId = 2,
                AdminLoginId = "84u232fhfehw889d0ufd",
                PasswordHash = "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A=="
            }
        );
    }
}