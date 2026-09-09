using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionService.Data.Configurations;

public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("Auctions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Seller)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Winner)
            .HasMaxLength(100);

        builder.Property(a => a.ReservePrice)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.Property(a => a.AuctionEnd)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // 1-to-1 relationship: Auction has one Item, Item has one Auction
        builder.HasOne(a => a.Item)
            .WithOne(i => i.Auction)
            .HasForeignKey<Item>(i => i.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
