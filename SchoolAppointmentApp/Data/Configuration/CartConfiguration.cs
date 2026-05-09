using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configuration;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.CartId);

        builder.Property(c => c.CustomerId)
                .IsRequired();

        // One customer can only has one cart that is not ordered
        builder.HasIndex(c => c.CustomerId)
                .IsUnique()
                .HasFilter("\"Ordered\" = false");

        // Cart must have ordered status
        builder.Property(c => c.Ordered)
                .IsRequired();

        // 1 Cart has many CartItem
        builder.HasMany(c => c.CartProductList)
                .WithOne(c => c.Cart)
                .HasForeignKey(c => c.CartId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        // 1 customer & its teacher has many carts
        builder.HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
    }
}