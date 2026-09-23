using Contracts;
using Mapster;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class TransientSearchException(string message) : Exception(message);
public class AuctionCreatedHandler
{
    private static readonly HashSet<string> FailedOnce = [];
    public async Task Handle(AuctionCreated auctionCreated, MeilisearchClient meilisearchClient)
    {
        if(auctionCreated.Make == "fail-once")
        {
            if (FailedOnce.Add(auctionCreated.Id))
            {
                throw new TransientSearchException($"Simulated once failure for {auctionCreated.Id}");
            }
        }

        if (auctionCreated.Make == "fail-always")
        {
            throw new TransientSearchException($"Simulated always failure for {auctionCreated.Id}");
        }

        var item = auctionCreated.Adapt<Item>();

        var index = meilisearchClient.Index("items");
        
        var task = await index.AddDocumentsAsync([item], "id");
        await meilisearchClient.WaitForTaskAsync(task.TaskUid);
    }
}
