using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class OrderItemStatusConfiguration : IEntityTypeConfiguration<OrderItemStatus>
{
    public void Configure(EntityTypeBuilder<OrderItemStatus> builder)
    {
        builder.HasKey(ois => ois.StatusId);

        builder.Property(ois => ois.Status)
            .HasConversion<string>();

        builder.HasData(
            new { StatusId = 1, Status = OrderItemPossibleStatus.pending },
            new { StatusId = 2, Status = OrderItemPossibleStatus.received },
            new { StatusId = 3, Status = OrderItemPossibleStatus.cancelled }
        );
    }
}