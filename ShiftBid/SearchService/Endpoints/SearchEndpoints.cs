using Meilisearch;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/search", async ([AsParameters] SearchParams searchParams, MeilisearchClient client) =>
        {
            var index = client.Index("items");

            var query = new SearchQuery
            {
                HitsPerPage = Math.Min(50, searchParams.PageSize ?? 4),
                Page = searchParams.PageNumber ?? 1
            };

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Q = searchParams.SearchTerm;
            }

            var filters = new List<string>();
            
            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                filters.Add($"seller = '{searchParams.Seller}'");
            }
            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                filters.Add($"winner = '{searchParams.Winner}'");
            }

            if (searchParams.FilterBy == "finished")
            {
                filters.Add("status = 'Finished'");
            }
            else if (searchParams.FilterBy == "endingSoon")
            {
                filters.Add("status = 'Live'");
                query.Sort = ["auctionEnd:asc"];
            }
            else if (searchParams.FilterBy == "live")
            {
                filters.Add("status = 'Live'");
            }

            if (filters.Count != 0)
            {
                query.Filter = string.Join(" AND ", filters);
            }

            // If we are not already sorting by endingSoon, apply OrderBy
            if (searchParams.FilterBy != "endingSoon" && !string.IsNullOrEmpty(searchParams.OrderBy))
            {
                if (searchParams.OrderBy == "make")
                {
                    query.Sort = ["make:asc"];
                }
                else if (searchParams.OrderBy == "new")
                {
                    query.Sort = ["createdAt:desc"];
                }
                else if (searchParams.OrderBy == "endingSoon")
                {
                    query.Sort = ["auctionEnd:asc"];
                }
            }

            var result = await index.SearchAsync<Item>(query.Q, query);
            
            var paginatedResult = result as PaginatedSearchResult<Item>;

            var response = new
            {
                results = result.Hits,
                pageCount = paginatedResult?.TotalPages ?? 1,
                totalCount = paginatedResult?.TotalHits ?? result.Hits.Count()
            };

            return Results.Ok(response);
        });

        app.MapGet("/api/search/{id}", async (string id, MeilisearchClient client) =>
        {
            try
            {
                var index = client.Index("items");
                var item = await index.GetDocumentAsync<Item>(id);
                return Results.Ok(item);
            }
            catch (MeilisearchApiError ex) when (ex.Code == "document_not_found")
            {
                return Results.NotFound();
            }
        });
    }
}
