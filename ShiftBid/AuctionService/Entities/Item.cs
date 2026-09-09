namespace AuctionService.Entities;

public class Item
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // Navigation property
    public Auction? Auction { get; set; }
    public string AuctionId { get; set; } = string.Empty;
}
