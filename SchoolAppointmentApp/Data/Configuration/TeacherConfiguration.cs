using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.TeacherId);

        builder.HasIndex(t => t.TeacherId);

        builder.Property(t => t.TeacherId)
                .IsRequired()
                .HasMaxLength(10)
                .ValueGeneratedNever();

        // Defualt to 0 if null
        builder.Property(t => t.Points)
                .HasDefaultValue(0);

        builder.HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.UserId)
                .IsRequired();

    }
}