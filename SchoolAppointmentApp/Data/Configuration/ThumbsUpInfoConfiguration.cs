using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class ThumbsUpInfoConfiguration : IEntityTypeConfiguration<ThumbsUpInfo>
{
    public void Configure(EntityTypeBuilder<ThumbsUpInfo> builder)
    {
        builder.HasKey(tui => tui.ThumbsUpInfoId);

        // A thumbs up info can only represent one post or reply
        builder.ToTable(tui => tui.HasCheckConstraint(
            "CK_ReplyIdAndPostIdCoExist",
            "(\"MainPostId\" IS NULL AND \"ReplyId\" IS NOT NULL)" +
            "OR (\"ReplyId\" IS NULL AND \"MainPostId\" IS NOT NULL)"
        ));

        // Uniquely identify
        builder.HasIndex(tui => new { tui.MainPostId, tui.UserId });
        builder.HasIndex(tui => new { tui.ReplyId, tui.UserId });

        builder.HasOne(tui => tui.MainPost)
                .WithMany(mp => mp.ThumbsUpInfos)
                .HasForeignKey(tui => tui.MainPostId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tui => tui.Reply)
                .WithMany(r => r.ThumbsUpInfos)
                .HasForeignKey(tui => tui.ReplyId)
                .OnDelete(DeleteBehavior.Restrict);

        // Is Required
        builder.Property(tui => tui.UserId).IsRequired();
        builder.Property(tui => tui.Thumbed).IsRequired();
    }
}