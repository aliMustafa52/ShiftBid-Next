using Contracts;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class BidPlacedHandler(MeilisearchClient meilisearchClient)
{
    public async Task Handle(BidPlaced message)
    {
        var auction = await meilisearchClient.Index("items")
                .GetDocumentAsync<Item>(message.AuctionId)
            ?? throw new InvalidOperationException("Auction not found");

        if(message.BidStatus.Contains("Accepted") 
            && message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = message.Amount;
            await meilisearchClient.Index("items")
            .UpdateDocumentsAsync([auction]);
        }
    }
}
