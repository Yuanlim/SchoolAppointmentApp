using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class FriendRequestStatusConfiguration : IEntityTypeConfiguration<FriendRequestStatus>
{
    public void Configure(EntityTypeBuilder<FriendRequestStatus> builder)
    {
        builder.HasKey(fs => fs.StatusId);

        builder.Property(fs => fs.FriendRequestPossibleStatus)
          .HasConversion<string>();

        builder.HasData(
            new { StatusId = 1, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Pending },
            new { StatusId = 2, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Denied },
            new { StatusId = 3, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Accepted }
        );
    }
}