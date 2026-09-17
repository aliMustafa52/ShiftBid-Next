using Meilisearch;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration, MeilisearchClient meilisearchClient)
{
    public async Task<List<Item>> GetItemsForSearch()
    {
        var searchQuery = new SearchQuery
        {
            Sort = ["updatedAt:desc"],
            Limit = 1
        };

        var search = await meilisearchClient.Index("items")
                            .SearchAsync<Item>(string.Empty, searchQuery);

        var lastUpdated = search.Hits.FirstOrDefault()?.UpdatedAt;

        var dateParam = lastUpdated?.ToString("O") ?? string.Empty;

        var items = await httpClient.GetFromJsonAsync<List<Item>>(
            $"{configuration["AuctionSvcUrl"]}/api/auctions?date={Uri.EscapeDataString(dateParam)}");

        return items ?? [];
    }
}
