using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class SchoolPrincipalConfiguration : IEntityTypeConfiguration<SchoolPrincipal>
{
    public void Configure(EntityTypeBuilder<SchoolPrincipal> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.PasswordHash).IsRequired();
        builder.Property(sp => sp.PrincipalId).IsRequired();

        builder.HasData(
            new
            {
                Id = 1,
                PrincipalId = "L123456",
                PasswordHash = "AQAAAAIAAYagAAAAEMRaGtjqdh0fBDTRhUT6cyIJME6FyreJVOKsdXOSOhJ6ifmqjbg1I/4i5TTGP66x9A=="
            }
        );
    }
}