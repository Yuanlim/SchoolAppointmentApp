using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        // HasKey: set up PK
        builder.HasKey(a => a.AdminId);

        // builder.Property set property of data field 
        // in here is set to required field
        builder.Property(a => a.PasswordHash).IsRequired();
        builder.Property(a => a.AdminLoginId).IsRequired();

        // HasIndex creates faster search and 
        // the whole row set to uniquely different
        builder.HasIndex(a => a.AdminLoginId).IsUnique();

        // Create initial data
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