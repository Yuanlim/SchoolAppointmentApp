using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.CustomerId)
                .IsRequired();

        builder.HasIndex(o => o.CustomerId);

        // Many order maybe place by one teacher(as customer)
        // Relation in customerId
        builder.HasOne(o => o.Teacher)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

        // 1 order may have many items, with Fk of OrderId
        builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

        // 1 order status can be in pending, cancelled or received.
        // Many Orders can have its own independent status
        builder.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}