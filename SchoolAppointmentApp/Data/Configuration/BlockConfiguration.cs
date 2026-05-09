using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(b => b.RequestId);

        builder.HasIndex(b => new { b.ReceiverId, b.InitiatorId });

        // Each Block can only points to one Receiver (HasOne)
        // But the Receiver could have multiple blocks (WithMany)
        // And the relation between block with Receiver is the ReceiverId (Has ForeignKey)
        // When Receiver delete do not delete the block record (OnDelete)
        builder.HasOne(b => b.Receiver)
                .WithMany() // Receiver possibly has many Block request
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