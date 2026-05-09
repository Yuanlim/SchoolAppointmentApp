using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.HasKey(os => os.StatusId);

        // Saved it as status string
        builder.Property(os => os.Status)
                .HasConversion<string>();

        builder.HasData(
            new { StatusId = 1, Status = OrderPossibleStatus.pending },
            new { StatusId = 2, Status = OrderPossibleStatus.cancelled },
            new { StatusId = 3, Status = OrderPossibleStatus.received },
            new { StatusId = 4, Status = OrderPossibleStatus.mix } // Received some but cancelled some.
        );
    }
}