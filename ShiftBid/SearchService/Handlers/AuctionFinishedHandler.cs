using Contracts;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class AuctionFinishedHandler(MeilisearchClient meilisearchClient)
{

    public async Task Handle(AuctionFinished message)
    {
        var auction = await meilisearchClient.Index("items")
                .GetDocumentAsync<Item>(message.AuctionId)
            ?? throw new InvalidOperationException("Auction not found");


        if (message.ItemSold)
        {
            auction.Winner = message.Winner;
            auction.SoldAmount = message.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice
            ? "Finished"
            : "ReserveNotMet";

        await meilisearchClient.Index("items")
            .UpdateDocumentsAsync([auction]);
    }
}
