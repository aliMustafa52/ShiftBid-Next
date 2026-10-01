using AuctionService.Data;
using Contracts;

namespace AuctionService.Handlers;

public class AuctionFinishedHandler(AuctionDbContext dbContext)
{
    private readonly AuctionDbContext _dbContext = dbContext;

    public async Task Handle(AuctionFinished message)
    {
        var auction = await _dbContext.Auctions
                .FindAsync(message.AuctionId) 
            ?? throw new InvalidOperationException("Auction not found");


        if (message.ItemSold)
        {
            auction.Winner = message.Winner;
            auction.SoldAmount = message.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice
            ? Entities.Status.Finished
            : Entities.Status.ReserveNotMet;

        await _dbContext.SaveChangesAsync();
    }
}
