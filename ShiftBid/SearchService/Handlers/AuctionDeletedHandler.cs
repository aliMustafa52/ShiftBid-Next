using Contracts;
using Meilisearch;

namespace SearchService.Handlers;

public class AuctionDeletedHandler
{
    public async Task Handle(AuctionDeleted message, MeilisearchClient meilisearchClient)
    {
        var index = meilisearchClient.Index("items");
        
        var task = await index.DeleteOneDocumentAsync(message.Id);
        await meilisearchClient.WaitForTaskAsync(task.TaskUid);
    }
}
