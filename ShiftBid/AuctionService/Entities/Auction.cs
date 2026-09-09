namespace AuctionService.Entities;

public class Auction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int ReservePrice { get; set; } = 0;
    public string Seller { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentHighBid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AuctionEnd { get; set; }
    public Status Status { get; set; } = Status.Live;

    // Navigation property
    public Item Item { get; set; } = null!;
}
