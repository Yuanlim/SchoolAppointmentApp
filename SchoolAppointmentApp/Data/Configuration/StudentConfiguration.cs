using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Setting Primary key
        builder.HasKey(s => s.StudentId);

        // (Often used) Faster Search
        builder.HasIndex(s => s.StudentId);

        // StudentId is required, maximum length under 30 
        // and should not be auto generated
        builder.Property(s => s.StudentId)
                .IsRequired()
                .HasMaxLength(10)
                .ValueGeneratedNever();

        // Each student represent one class
        builder.HasOne(s => s.SchoolClass)
                .WithMany()
                .HasForeignKey(s => s.ClassId);

        // Student must have one class
        builder.Property(s => s.ClassId)
                .IsRequired();

        // One to one with user table
        builder.HasOne(s => s.User)
                .WithOne(user => user.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        // Each student must required a UserId
        builder.Property(s => s.UserId)
                .IsRequired();


    }
}