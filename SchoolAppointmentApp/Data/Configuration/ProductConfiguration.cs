using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Setting Primary key
        builder.HasKey(p => p.ProductId);

        // (Often used) Faster Search
        builder.HasIndex(p => p.ProductId);

        // Default to 0 if empty (scraped)
        // builder.Property(p => p.PointCost).HasDefaultValue(0);

        builder.Property(p => p.ProductName).IsRequired();

        // Initialize some data
        builder.HasData(
            new
            {
                ProductId = 1,
                ProductName = "item",
                ProductImageRoot = "../Example1",
                Description = "Just a item",
                AvailableQuantity = 5,
                PointCost = 1
            },
            new
            {
                ProductId = 2,
                ProductName = "snack",
                ProductImageRoot = "../Example2",
                Description = "Just a snack",
                AvailableQuantity = 30,
                PointCost = 2
            },
            new
            {
                ProductId = 3,
                ProductName = "tool",
                ProductImageRoot = "../Example3",
                Description = "Just a tool ",
                AvailableQuantity = 10,
                PointCost = 3
            }
        );
    }
}