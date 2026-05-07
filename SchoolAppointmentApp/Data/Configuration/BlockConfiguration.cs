using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(b => b.RequestId);

        builder.HasIndex(b => new { b.ReceiverId, b.InitiatorId });

        builder.HasOne(b => b.Receiver)
                .WithMany() // Receiver possibilly has many Block request
                .HasForeignKey(b => b.ReceiverId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Initiator)
                .WithMany()
                .HasForeignKey(b => b.InitiatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
    }
}