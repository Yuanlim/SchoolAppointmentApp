using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.HasKey(fr => fr.RequestId);

        builder.HasIndex(fr => new { fr.ReceiverId, fr.InitiatorId });

        builder.HasOne(fr => fr.Receiver)
                .WithMany() // Receiver possibly has many friend request
                .HasForeignKey(fr => fr.ReceiverId) // FK keys of FriendStatus Receiver id
                .IsRequired();

        builder.HasOne(fr => fr.Initiator)
                .WithMany()
                .HasForeignKey(fr => fr.InitiatorId)
                .IsRequired();

        builder.HasOne(fr => fr.FriendRequestStatus)
                .WithMany()
                .HasForeignKey(fr => fr.StatusId)
                .IsRequired();
    }
}