using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        // Primary key of SchoolClass
        builder.HasKey(sc => sc.ClassId);

        // Each sc must have class name
        builder.Property(sc => sc.ClassName)
                .IsRequired();

        // Each class name is uniquely different
        builder.HasIndex(sc => sc.ClassName)
                .IsUnique();

        // Initialize some data
        builder.HasData(
            new { ClassId = 1, ClassName = "電通一甲" },
            new { ClassId = 2, ClassName = "電通二甲" },
            new { ClassId = 3, ClassName = "電通三甲" }
        );
    }
}