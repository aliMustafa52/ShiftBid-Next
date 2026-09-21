using Contracts;
using Mapster;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class AuctionCreatedHandler
{
    public async Task Handle(AuctionCreated auctionCreated, MeilisearchClient meilisearchClient)
    {
        var item = auctionCreated.Adapt<Item>();

        var index = meilisearchClient.Index("items");
        
        var task = await index.AddDocumentsAsync([item], "id");
        await meilisearchClient.WaitForTaskAsync(task.TaskUid);
    }
}
