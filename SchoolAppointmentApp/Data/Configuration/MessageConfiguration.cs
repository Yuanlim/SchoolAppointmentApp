using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.MessageId);

        builder.HasIndex(m => new { m.SenderId, m.ReceiverId });

        builder.HasOne(m => m.Sender)
              .WithMany()
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Receiver)
              .WithMany()
              .HasForeignKey(m => m.ReceiverId)
              .OnDelete(DeleteBehavior.Restrict);

        // Content || Audio || Image must have at least one
        builder.ToTable(t => t.HasCheckConstraint(
            "CheckMessageHasExactlyOneTypeOfContent", // Name
            @"(
                (CASE WHEN ""Content"" IS NOT NULL AND length(trim(""Content"")) > 0 THEN 1 ELSE 0 END) +
                (CASE WHEN ""AudioMessageRoot"" IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""ImageMessageRoot"" IS NOT NULL THEN 1 ELSE 0 END)
            ) = 1"
        ));
    }
}