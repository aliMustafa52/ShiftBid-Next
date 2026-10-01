using AuctionService.Data;
using Contracts;

namespace AuctionService.Handlers;

public class BidPlacedHandler(AuctionDbContext dbContext)
{
    private readonly AuctionDbContext _dbContext = dbContext;

    public async Task Handle(BidPlaced message)
    {
        var auction = await _dbContext.Auctions.FindAsync(message.AuctionId)
            ?? throw new InvalidOperationException("Auction not found");

        if(message.BidStatus.Contains("Accepted") 
            && message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = message.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }
}
