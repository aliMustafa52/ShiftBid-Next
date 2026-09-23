using Contracts;
using Meilisearch;

namespace SearchService.Handlers;

public class AuctionUpdatedHandler
{
    public async Task Handle(AuctionUpdated message, MeilisearchClient meilisearchClient)
    {
        var index = meilisearchClient.Index("items");
        
        var task = await index.UpdateDocumentsAsync([message], "id");
        await meilisearchClient.WaitForTaskAsync(task.TaskUid);
    }
}
