using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
{
	public void Configure(EntityTypeBuilder<Reply> builder)
	{
		builder.HasKey(r => r.ReplyId);

		builder.HasOne(r => r.MainPost)
				.WithMany(mp => mp.Replies)
				.HasForeignKey(r => r.MainPostId)
				.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Restrict);

		builder.Property(r => r.Content)
				.IsRequired();
		builder.Property(r => r.PostDateTime)
				.IsRequired();
		builder.Property(r => r.UserId)
				.IsRequired();
		builder.Property(r => r.MainPostId)
				.IsRequired();
	}
}