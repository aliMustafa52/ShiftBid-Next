using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionService.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Make)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Model)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Year)
            .IsRequired();

        builder.Property(i => i.Color)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(i => i.Mileage)
            .IsRequired();

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(i => i.Auction)
            .WithOne(a => a.Item)
            .HasForeignKey<Item>(i => i.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
