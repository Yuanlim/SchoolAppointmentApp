using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class MainPostConfiguration : IEntityTypeConfiguration<MainPost>
{
    public void Configure(EntityTypeBuilder<MainPost> builder)
    {
        builder.HasKey(mp => mp.MainPostId);

        builder.HasOne(mp => mp.Student)
                .WithMany()
                .HasForeignKey(mps => mps.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mp => mp.Replies)
                .WithOne()
                .HasForeignKey(r => r.MainPostId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Property(mp => mp.Content)
                .IsRequired();
        builder.Property(mp => mp.PostDateTime)
                .IsRequired();
    }
}