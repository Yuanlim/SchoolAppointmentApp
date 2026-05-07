using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.RequestId);

        builder.HasIndex(r => new { r.ReceiverId, r.InitiatorId });

        builder.HasOne(r => r.Receiver)
                .WithMany() // Receiver possibilly has many Report request
                .HasForeignKey(r => r.ReceiverId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Initiator)
                .WithMany()
                .HasForeignKey(r => r.InitiatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
    }
}