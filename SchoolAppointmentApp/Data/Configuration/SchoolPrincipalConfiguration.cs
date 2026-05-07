using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

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
                PasswordHash = "AQAAAAIAAYagAAAAEOe6noMHGXGrXzbCSkir9wB2m2z8GwLZTUp69XY2CT9Bpe4dwpTh29iOYbVBPp2dNw=="
            }
        );
    }
}